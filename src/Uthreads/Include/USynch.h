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

#ifndef USYNCH_DLL
#define USYNCH_API __declspec(dllimport)
#else
#define USYNCH_API __declspec(dllexport)
#endif

typedef struct Event {
	BOOL Signaled;
	LIST_ENTRY Waiters;
} EVENT, *PEVENT;

#ifdef __cplusplus
extern "C" {
#endif

USYNCH_API
VOID EventInit (PEVENT Event, BOOL Signaled);

FORCEINLINE
BOOL EventValue (PEVENT Event) {
	return Event->Signaled; 
}

USYNCH_API
VOID EventWait (PEVENT Event);

USYNCH_API
VOID EventSignal (PEVENT Event);


// Exercicio 1

typedef struct _CountDownLatch {
	int counter;
	LIST_ENTRY Waiters;
} CountDownLatch, *PCountDownLatch;

USYNCH_API
VOID CountDownLatchInit (PCountDownLatch CountDownLatch, int init_counter);

USYNCH_API
VOID WaitLatch (PCountDownLatch CountDownLatch);

USYNCH_API
VOID SignalLatch (PCountDownLatch CountDownLatch);


#ifdef __cplusplus
} // extern "C"
#endif
