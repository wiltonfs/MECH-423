// Int buffer (queue) functions for MECH 423
// Felix Wilton
// Oct 2024

// Inclusion guard
#ifndef FSWINTBUFFER_INCLUDED
#define FSWINTBUFFER_INCLUDED





// Change here if you want signed ints
typedef unsigned int IntBufferValue;
#define INTBUFFER_SIZE 10

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

typedef struct {
    volatile IntBufferValue buffer[INTBUFFER_SIZE];
    volatile unsigned int head;  // Points to the next position to add
    volatile unsigned int tail;  // Points to the next position to pop
    volatile unsigned int count; // Keeps track of how many elements are in the buffer
} CircularIntBuffer;

bool IB_addValue(CircularIntBuffer *ib, IntBufferValue value);
// Adds a value to the circular buffer. 
// Returns true if successful, false otherwise.

bool IB_addStringValue(CircularIntBuffer *ib, const unsigned char *str);
// Adds a series of values to the circular buffer. 
// Returns true if successful, false otherwise.

IntBufferValue IB_popValue(CircularIntBuffer *ib);
// Pops the oldest value in the buffer.
// Returns 0 if buffer is empty.

bool IB_popValueSAFE(CircularIntBuffer *ib, IntBufferValue *valueOut);
// Pops the oldest value in the buffer into the valueOut parameter. 
// Returns true if successful, false otherwise.

bool IB_isEmpty(CircularIntBuffer *ib);
// Returns true if the buffer is empty, false otherwise.

bool IB_isFull(CircularIntBuffer *ib);
// Returns true if the buffer is full, false otherwise.

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// Adds a value to the circular buffer. Returns true if successful, false otherwise
bool IB_addValue(CircularIntBuffer *ib, IntBufferValue value) {
    if (IB_isFull(ib)) {
        // Buffer is full, cannot add more values
        return false;
    }
    ib->buffer[ib->head] = value;               // Add value to the head
    ib->head = (ib->head + 1) % INTBUFFER_SIZE;    // Increment head with wrap-around
    ib->count++;                                // Increment count
    return true;
}

bool IB_addStringValue(CircularIntBuffer *ib, const unsigned char *str) {
    while (*str != '\0') { // Continue until we reach the null terminator
        if (!IB_addValue(ib, *str)) {
            // If the buffer gets full, return false
            return false;
        }
        str++; // Move to the next character in the string
    }
    return true; // successfully added all characters
}

IntBufferValue IB_popValue(CircularIntBuffer *ib) {
    if (IB_isEmpty(ib)) {
        return 0;
    }
    IntBufferValue out = ib->buffer[ib->tail];     // Get value from the tail
    ib->tail = (ib->tail + 1) % INTBUFFER_SIZE;        // Increment tail with wrap-around
    ib->count--;                                    // Decrement count
    return out;
}

bool IB_popValueSAFE(CircularIntBuffer *ib, IntBufferValue *valueOut) {
    if (IB_isEmpty(ib)) {
        // Buffer is empty, cannot pop any values
        return false;
    }
    *valueOut = IB_popValue(ib);    // Pop value
    return true;
}

bool IB_isEmpty(CircularIntBuffer *ib) {
    return ib->count == 0;
}

bool IB_isFull(CircularIntBuffer *ib) {
    return ib->count == INTBUFFER_SIZE;
}











// Inclusion guard
#endif  // FSWINTBUFFER_INCLUDED