// Char buffer functions for MECH 423
// Felix Wilton
// Oct 2024

// Inclusion guard
#ifndef FSWCHARBUFFER_INCLUDED
#define FSWCHARBUFFER_INCLUDED






typedef unsigned char CharBufferValue;
#define BUFFER_SIZE 50

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

typedef struct {
    volatile CharBufferValue buffer[BUFFER_SIZE];
    volatile unsigned int head;  // Points to the next position to add
    volatile unsigned int tail;  // Points to the next position to pop
    volatile unsigned int count; // Keeps track of how many elements are in the buffer
} CircularCharBuffer;

bool addValue(CircularCharBuffer *cb, CharBufferValue value);
// Adds a value to the circular buffer. 
// Returns true if succesful, false otherwise.

bool addStringValue(CircularCharBuffer *cb, const unsigned char *str);
// Adds a series of values to the circular buffer. 
// Returns true if succesful, false otherwise.

CharBufferValue popValue(CircularCharBuffer *cb);
// Pops the oldest value in the buffer.
// Returns 0 if buffer is empty.

bool popValueSAFE(CircularCharBuffer *cb, CharBufferValue *valueOut);
// Pops the oldest value in the buffer into the valueOut parameter. 
// Returns true if succesful, false otherwise.

bool isEmpty(CircularCharBuffer *cb);
// Returns true if the buffer is empty, false otherwise.

bool isFull(CircularCharBuffer *cb);
// Returns true if the buffer is full, false otherwise.

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// Adds a value to the circular buffer. Returns true if succesful, false otherwise
bool addValue(CircularCharBuffer *cb, CharBufferValue value) {
    if (cb->count == BUFFER_SIZE) {
        // Buffer is full, cannot add more values
        return false;
    }
    cb->buffer[cb->head] = value;               // Add value to the head
    cb->head = (cb->head + 1) % BUFFER_SIZE;    // Increment head with wrap-around
    cb->count++;                                // Increment count
    return true;
}

bool addStringValue(CircularCharBuffer *cb, const unsigned char *str) {
    while (*str != '\0') { // Continue until we reach the null terminator
        if (!addValue(cb, *str)) {
            // If the buffer is full, return false
            return false;
        }
        str++; // Move to the next character in the string
    }
    return true; // Successfully added all characters
}

CharBufferValue popValue(CircularCharBuffer *cb) {
    if (isEmpty(cb)) {
        return 0;
    }
    CharBufferValue out = cb->buffer[cb->tail];     // Get value from the tail
    cb->tail = (cb->tail + 1) % BUFFER_SIZE;        // Increment tail with wrap-around
    cb->count--;                                    // Decrement count
    return out;
}

bool popValueSAFE(CircularCharBuffer *cb, CharBufferValue *valueOut) {
    if (isEmpty(cb)) {
        // Buffer is empty, cannot pop any values
        return false;
    }
    *valueOut = cb->buffer[cb->tail];   // Get value from the tail
    cb->tail = (cb->tail + 1) % BUFFER_SIZE;    // Increment tail with wrap-around
    cb->count--;                                // Decrement count
    return true;
}

bool isEmpty(CircularCharBuffer *cb) {
    return cb->count == 0;
}

bool isFull(CircularCharBuffer *cb) {
    return cb->count == BUFFER_SIZE;
}











// Inclusion guard
#endif  // FSWCHARBUFFER_INCLUDED