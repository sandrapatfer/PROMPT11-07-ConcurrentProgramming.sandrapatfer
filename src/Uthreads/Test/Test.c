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

#include <stdio.h>
#include <assert.h>
#include "UThread.h"
#include "USynch.h"

/*
 *   Producer: reads lines from the input, placing them on the common buffer line.
 *   Filter:   decides if the line in the common buffer shall be passed to the Consumer.
 *   Consumer: sends the line in the common buffer to the output.
 *
 *   FreeEvent:   the common buffer (line) is free
 *   LineEvent:   a new line is available in the common buffer
 *   ResultEvent: the line in the common buffer has passed the filter criterion
 *   
 *                  +---+
 *      +------(W)--| ? |<--(S)---------------------------------+
 *      |           +---+<--(S)-----+                           |
 *      V         FreeEvent         |                           |
 *   +-----+                     +-----+                     +-----+
 *   |     |        +---+        |     |        +---+        |     |
 *   |  P  |--(S)-->| ? |--(W)-->|  F  |--(S)-->| ? |--(W)-->|  F  |
 *   |     |        +---+        |     |        +---+        |     |
 *   +-----+      LineEvent      +-----+      ResultEvent    +-----+
 *  Producer:                   Filter:                     Consumer:
 *  ReaderThread                FilterThread                PrinterThread
 *                                                          or
 *                                                          WriterThread
 */

int  end;          // TRUE at the end of the input file
char line[256];    // common buffer to hold a line of text

EVENT FreeEvent;   // the common buffer (line) is free
EVENT LineEvent;   // a new line is available in the common buffer
EVENT ResultEvent; // the line in the common buffer has passed the filter criterion

VOID Prepare() {
	end = 0;
	EventInit(&FreeEvent, TRUE);
	EventInit(&LineEvent, FALSE);
	EventInit(&ResultEvent, FALSE);
}

VOID ReaderThread(UT_ARGUMENT inFile) {
	FILE * f = fopen((const char *)inFile, "r");
	assert(f != NULL);
	do {
		EventWait(&FreeEvent);
		if (fgets(line, 256, f) == NULL) end = 1;
		EventSignal(&LineEvent);
	} while (end == 0);
	fclose(f);
}

VOID FilterThread(UT_ARGUMENT arg) {
	char c = (char)arg;
	do {
		EventWait(&LineEvent);
		if (end == 1 || strchr(line, c) != NULL) {
			EventSignal(&ResultEvent);
		} else {
			EventSignal(&FreeEvent);
		}
	} while (end == 0);
}

VOID WriterThread(UT_ARGUMENT outFile) {
	FILE * f = fopen((const char *)outFile, "w+");
	assert(f != NULL);
	do {
		EventWait(&ResultEvent);
		if (end == 0) fputs(line, f);
		EventSignal(&FreeEvent);
	} while (end == 0);
	fclose(f);
}

VOID PrinterThread(UT_ARGUMENT outFile) {
	do {
		EventWait(&ResultEvent);
		if (end == 0) printf("%s", line);
		EventSignal(&FreeEvent);
	} while (end == 0);
}

int main(int argc, const char * argv[]) {

	if (argc != 3) {
		fprintf(stderr, "use: %s input_file filter_char\n", argv[0]);
		exit(1);
	}

	UtInit();

	Prepare();

	UtCreate(ReaderThread, (UT_ARGUMENT)argv[1]);
	UtCreate(FilterThread, (UT_ARGUMENT)argv[2][0]);
	UtCreate(PrinterThread, (UT_ARGUMENT)0);
	
	UtRun();
	
	UtEnd();
	return 0;
}
