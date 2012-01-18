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

#pragma once

#include <Windows.h>
#include "UThread.h"
#include "List.h"

//
// The data structure representing the layout of a thread's execution context
// when saved in the stack.
//
typedef struct _UTHREAD_CONTEXT {
	ULONG EDI;
	ULONG ESI;
	ULONG EBX;
	ULONG EBP;
	VOID (*RetAddr)();
} UTHREAD_CONTEXT, *PUTHREAD_CONTEXT;

//
// The descriptor of a user thread, containing an intrusive link (through which
// the thread is linked in the ready queue), the thread's starting function and
// argument, the memory block used as the thread's stack and a pointer to the
// saved execution context.
//
typedef struct _UTHREAD {
	LIST_ENTRY       Link;
	UT_FUNCTION      Function;   
	UT_ARGUMENT      Argument; 
	PUCHAR           Stack;
	PUTHREAD_CONTEXT ThreadContext;
} UTHREAD, *PUTHREAD;

//
// The fixed stack size of a user thread.
//
#define STACK_SIZE (8 * 4096)
