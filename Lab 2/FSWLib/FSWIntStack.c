// Int stack functions for MECH 423
// Felix Wilton
// Oct 2024

// Inclusion guard
#ifndef FSWINTSTACK_INCLUDED
#define FSWINTSTACK_INCLUDED






typedef unsigned char IntStackValue;
#define INTSTACK_SIZE 15

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

typedef struct {
    volatile IntStackValue stack[INTSTACK_SIZE];
    volatile unsigned int head;  // Points to the next position to add
} IntStack;

bool IS_addValue(IntStack *is, IntStackValue value);
// Adds a value to the stack. 
// Returns true if successful, false otherwise.

bool IS_addStringValue(IntStack *is, const unsigned char *str);
// Adds a series of values to the stack. 
// Returns true if successful, false otherwise.

IntStackValue IS_popValue(IntStack *is);
// Pops the newest value in the stack.
// Returns 0 if stack is empty.

bool IS_popValueSAFE(IntStack *is, IntStackValue *valueOut);
// Pops the newest value in the stack into the valueOut parameter. 
// Returns true if successful, false otherwise.

bool IS_isEmpty(IntStack *is);
// Returns true if the stack is empty, false otherwise.

bool IS_isFull(IntStack *is);
// Returns true if the stack is full, false otherwise.

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

bool IS_addValue(IntStack *is, IntStackValue value) {
    if (IS_isFull(is)) {
        // Stack is full, cannot add more values
        return false;
    }
    is->stack[is->head] = value;                // Add value to the head
    is->head++;                                 // Increment head
    return true;
}

bool IS_addStringValue(IntStack *is, const unsigned char *str) {
    while (*str != '\0') { // Continue until we reach the null terminator
        if (!IS_addValue(is, *str)) {
            // If the stack gets full, return false
            return false;
        }
        str++; // Move to the next character in the string
    }
    return true; // successfully added all characters
}

IntStackValue IS_popValue(IntStack *is) {
    if (IS_isEmpty(is)) {
        return 0;
    }
    IntStackValue out = is->stack[is->head - 1];   // Get value from before the head
    is->head--;                                    // Decrement head
    return out;
}

bool IS_popValueSAFE(IntStack *is, IntStackValue *valueOut) {
    if (IS_isEmpty(is)) {
        // Stack is empty, cannot pop any values
        return false;
    }
    *valueOut = IS_popValue(is);   // Pop value
    return true;
}

bool IS_isEmpty(IntStack *is) {
    return is->head == 0;
}

bool IS_isFull(IntStack *is) {
    return is->head >= INTSTACK_SIZE;
}











// Inclusion guard
#endif  // FSWINTSTACK_INCLUDED