/////////////////////////////////////////////////////////////////
//
// CCISEL 
// 2007-2011
//
// UThread library:
//   User threads supporting cooperative multithreading.
//
// Authors:
//   Carlos Martins, João Trindade, Duarte Nunes, Jorge Martins
// 

#include <crtdbg.h>
#include "UThreadInternal.h"

//////////////////////////////////////
//
// UThread internal state variables.
//

//
// The number of existing user threads.
//
static
ULONG NumberOfThreads;

//
// The sentinel of the circular list linking the user threads that are
// currently schedulable. The next thread to run is retrieved from the
// head of this list.
//
static
LIST_ENTRY ReadyQueue;

//
// The currently executing thread.
//
static
PUTHREAD RunningThread;

//
// The user thread proxy of the underlying operating system thread. This
// thread is switched back in when there are no more runnable user threads,
// causing the scheduler to exit.
//
static
PUTHREAD MainThread;

////////////////////////////////////////////////
//
// Forward declaration of internal operations.
//

//
// The trampoline function that a user thread begins by executing, through
// which the associated function is called.
//
static
VOID InternalStart ();

//
// Performs a context switch from CurrentThread to NextThread.
// __fastcall sets the calling convention such that CurrentThread is in ECX
// and NextThread in EDX.
//
static
VOID __fastcall ContextSwitch (PUTHREAD CurrentThread, PUTHREAD NextThread);

//
// Frees the resources associated with CurrentThread and switches to NextThread.
// __fastcall sets the calling convention such that CurrentThread is in ECX
// and NextThread in EDX.
//
static
VOID __fastcall InternalExit (PUTHREAD Thread, PUTHREAD NextThread);

////////////////////////////////////////
//
// UThread inline internal operations.
//

//
// Returns and removes the first user thread in the ready queue. If the ready
// queue is empty, the main thread is returned.
//
static
FORCEINLINE
PUTHREAD ExtractNextReadyThread () {
	return IsListEmpty(&ReadyQueue) 
		 ? MainThread 
		 : CONTAINING_RECORD(RemoveHeadList(&ReadyQueue), UTHREAD, Link);
}

//
// Schedule a new thread to run
//
static
FORCEINLINE
VOID Schedule () {
	PUTHREAD NextThread;
    NextThread = ExtractNextReadyThread();
	ContextSwitch(RunningThread, NextThread);
}

///////////////////////////////
//
// UThread public operations.
//

//
// Initialize the scheduler.
// This function must be the first to be called. 
//
VOID UtInit() {
	InitializeListHead(&ReadyQueue);
}

//
// Cleanup all UThread internal resources.
//
VOID UtEnd() {
	/* (this function body was intentionally left empty) */
}

//
// Run the user threads. The operating system thread that calls this function
// performs a context switch to a user thread and resumes execution only when
// all user threads have exited.
//
VOID UtRun () {
	UTHREAD Thread; // Represents the underlying operating system thread.

	//
	// There can be only one scheduler instance running.
	//
	_ASSERTE(RunningThread == NULL);

	//
	// At least one user thread must have been created before calling run.
	//
	if (IsListEmpty(&ReadyQueue)) {
		return;
	}

	//
	// Switch to a user thread.
	//
	MainThread = &Thread;
	RunningThread = MainThread;
	Schedule();
 
	//
	// When we get here, there are no more runnable user threads.
	//
	_ASSERTE(IsListEmpty(&ReadyQueue));
	_ASSERTE(NumberOfThreads == 0);

	//
	// Allow another call to UtRun().
	//
	RunningThread = NULL;
	MainThread = NULL;
}


//
// Creates a user thread to run the specified function. The thread is placed
// at the end of the ready queue.
//
HANDLE UtCreate (UT_FUNCTION Function, UT_ARGUMENT Argument) {
	PUTHREAD Thread;
	
	//
	// Dynamically allocate an instance of UTHREAD and the associated stack.
	//
	Thread = (PUTHREAD) malloc(sizeof (UTHREAD));
	Thread->Stack = (PUCHAR) malloc(STACK_SIZE);
	_ASSERTE(Thread != NULL && Thread->Stack != NULL);

	//
	// Zero the stack for emotional confort.
	//
	memset(Thread->Stack, 0, STACK_SIZE);

	//
	// Memorize Function and Argument for use in InternalStart.
	//
	Thread->Function = Function;
	Thread->Argument = Argument;

	//
	// Map an UTHREAD_CONTEXT instance on the thread's stack.
	// We'll use it to save the initial context of the thread.
	//
	// +------------+
	// | 0x00000000 |    <- Highest word of a thread's stack space
	// +============+       (needs to be set to 0 for Visual Studio to
	// |  RetAddr   | \     correctly present a thread's call stack).
	// +------------+  |
	// |    EBP     |  |
	// +------------+  |
	// |    EBX     |   >   Thread->ThreadContext mapped on the stack.
	// +------------+  |
	// |    ESI     |  |
	// +------------+  |
	// |    EDI     | /  <- The stack pointer will be set to this address
	// +============+       at the next context switch to this thread.
	// |            | \
	// +------------+  |
	// |     :      |  |
	//       :          >   Remaining stack space.
	// |     :      |  |
	// +------------+  |
	// |            | /  <- Lowest word of a thread's stack space
	// +------------+       (Thread->Stack always points to this location).
	//

	Thread->ThreadContext = (PUTHREAD_CONTEXT) (Thread->Stack +
		STACK_SIZE - sizeof (ULONG) - sizeof (UTHREAD_CONTEXT));

	//
	// Set the thread's initial context by initializing the values of EDI,
	// EBX, ESI and EBP (must be zero for Visual Studio to correctly present
	// a thread's call stack) and by hooking the return address.
	// 
	// Upon the first context switch to this thread, after popping the dummy
	// values of the "saved" registers, a ret instruction will place the
	// address of InternalStart on EIP.
	//
	Thread->ThreadContext->EDI = 0x33333333;
	Thread->ThreadContext->EBX = 0x11111111;
	Thread->ThreadContext->ESI = 0x22222222;
	Thread->ThreadContext->EBP = 0x00000000;									  
	Thread->ThreadContext->RetAddr = InternalStart;

	//
	// Ready the thread.
	//
	NumberOfThreads += 1;
	UtActivate((HANDLE)Thread);
	
	return (HANDLE)Thread;
}

//
// Terminates the execution of the currently running thread. All associated
// resources are released after the context switch to the next ready thread.
//
VOID UtExit () {
	NumberOfThreads -= 1;	
	InternalExit(RunningThread, ExtractNextReadyThread());
	_ASSERTE(!"Supposed to be here!");
}

//
// Relinquishes the processor to the first user thread in the ready queue.
// If there are no ready threads, the function returns immediately.
//
VOID UtYield () {
	if (!IsListEmpty(&ReadyQueue)) {
		InsertTailList(&ReadyQueue, &RunningThread->Link);
		Schedule();
	}
}

//
// Returns a HANDLE to the executing user thread.
//
HANDLE UtSelf () {
	return (HANDLE)RunningThread;
}

//
// Halts the execution of the current user thread.
//
VOID UtDeactivate() {
	Schedule();
}


//
// Places the specified user thread at the end of the ready queue, where it
// becomes eligible to run.
//
VOID UtActivate (HANDLE ThreadHandle) {
	InsertTailList(&ReadyQueue, &((PUTHREAD)ThreadHandle)->Link);
}

///////////////////////////////////////
//
// Definition of internal operations.
//

//
// The trampoline function that a user thread begins by executing, through
// which the associated function is called.
//
VOID InternalStart () {
	RunningThread->Function(RunningThread->Argument);
	UtExit(); 
}

//
// Performs a context switch from CurrentThread to NextThread.
// __fastcall sets the calling convention such that CurrentThread is in ECX and NextThread in EDX.
// __declspec(naked) directs the compiler to omit any prologue or epilogue.
//
__declspec(naked) 
VOID __fastcall ContextSwitch (PUTHREAD CurrentThread, PUTHREAD NextThread) {
	__asm {

		//
		// Switch out the running CurrentThread, saving the execution context on the thread's own stack.   
		// The return address is atop the stack, having been placed there by the call to this function.
		//
		push	ebp
		push	ebx
		push	esi
		push	edi

		//
		// Save ESP in CurrentThread->ThreadContext.
		//
		mov		dword ptr [ecx].ThreadContext, esp

		//
		// Set NextThread as the running thread.
		//
		mov     RunningThread, edx

		//
		// Load NextThread's context, starting by switching to its stack, where the registers are saved.
		//
		mov		esp, dword ptr [edx].ThreadContext
		pop		edi
		pop		esi
		pop		ebx
		pop		ebp
	
		//
		// Jump to the return address saved on NextThread's stack when the function was called.
		//
		ret
	}
}

//
// Frees the resources associated with Thread.
// __fastcall sets the calling convention such that Thread is in ECX.
//

static
VOID __fastcall CleanupThread (PUTHREAD Thread) {
	free(Thread->Stack);
	free(Thread);
}

//
// Frees the resources associated with CurrentThread and switches to NextThread.
// __fastcall sets the calling convention such that CurrentThread is in ECX and NextThread in EDX.
// __declspec(naked) directs the compiler to omit any prologue or epilogue.
//
__declspec(naked)
VOID __fastcall InternalExit (PUTHREAD CurrentThread, PUTHREAD NextThread) {
	__asm {

		//
		// Set NextThread as the running thread.
		//
		mov     RunningThread, edx
		
		//
		// Load NextThread's stack pointer before calling CleanupThread(): making the call while
		// using CurrentThread's stack would mean using the same memory being freed -- the stack.
		//
		mov		esp, dword ptr [edx].ThreadContext

		call    CleanupThread

		//
		// Finish switching in NextThread.
		//
		pop		edi
		pop		esi
		pop		ebx
		pop		ebp
		ret
	}
}
