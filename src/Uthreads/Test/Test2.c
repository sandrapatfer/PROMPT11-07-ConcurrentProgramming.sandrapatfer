#include <stdio.h>
#include <assert.h>
#include "UThread.h"
#include "USynch.h"


EVENT FreeEvent;   // the common buffer (line) is free


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
