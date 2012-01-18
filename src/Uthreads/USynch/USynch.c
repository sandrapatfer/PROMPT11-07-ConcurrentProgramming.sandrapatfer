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

#include "USynch.h"
#include "UThread.h"
#include "WaitBlock.h"

VOID EventInit (PEVENT Event, BOOL Signaled) {
	Event->Signaled = Signaled;
	InitializeListHead(&Event->Waiters);
}

VOID EventWait (PEVENT Event) {
	if (Event->Signaled == TRUE) {
		Event->Signaled = FALSE;
	} else {
		WAIT_BLOCK WaitBlock;
		WaitBlock.Thread = UtSelf();
		InsertTailList(&Event->Waiters, &WaitBlock.Link);
		UtDeactivate();
	}
}

VOID EventSignal (PEVENT Event) {
	if (IsListEmpty(&Event->Waiters)) {
		Event->Signaled = TRUE;
	} else {
		PWAIT_BLOCK WaitBlockPtr =
			CONTAINING_RECORD(RemoveHeadList(&Event->Waiters), WAIT_BLOCK, Link);
		UtActivate(WaitBlockPtr->Thread);
	}
}


// Exercicio 1

VOID CountDownLatchInit (PCountDownLatch CountDownLatch, int init_counter) {
	CountDownLatch->counter = init_counter;
	InitializeListHead(&CountDownLatch->Waiters);
}

VOID WaitLatch (PCountDownLatch CountDownLatch) {
	// coloca a thread invocante em espera até que o contador interno tenha o valor zero

	if (CountDownLatch->counter > 0) {
		WAIT_BLOCK WaitBlock;
		WaitBlock.Thread = UtSelf();
		InsertTailList(&CountDownLatch->Waiters, &WaitBlock.Link);
		UtDeactivate();
	}
}

VOID SignalLatch (PCountDownLatch CountDownLatch) {
	// decrementa o contador interno, desbloqueando todas as threads em espera se este chegar a zero

	if (CountDownLatch->counter == 0) {
	}

	CountDownLatch->counter--;
	if (CountDownLatch->counter == 0) {
		while (!IsListEmpty(&CountDownLatch->Waiters)) {
			PWAIT_BLOCK WaitBlockPtr = CONTAINING_RECORD(RemoveHeadList(&CountDownLatch->Waiters), WAIT_BLOCK, Link);
			UtActivate(WaitBlockPtr->Thread);
		}
	}
}