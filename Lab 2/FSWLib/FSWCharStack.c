// Char stack functions for MECH 423
// Felix Wilton
// Oct 2024

// Inclusion guard
#ifndef FSWCHARSTACK_INCLUDED
#define FSWCHARSTACK_INCLUDED






typedef unsigned char CharStackValue;
#define CHARSTACK_SIZE 50

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

typedef struct {
    volatile CharStackValue stack[CHARSTACK_SIZE];
    volatile unsigned int head;  // Points to the next position to add
} CharStack;

bool CS_addValue(CharStack *cs, CharStackValue value);
// Adds a value to the stack. 
// Returns true if successful, false otherwise.

bool CS_addStringValue(CharStack *cs, const unsigned char *str);
// Adds a series of values to the stack. 
// Returns true if successful, false otherwise.

CharStackValue CS_popValue(CharStack *cs);
// Pops the newest value in the stack.
// Returns 0 if stack is empty.

bool CS_popValueSAFE(CharStack *cs, CharStackValue *valueOut);
// Pops the newest value in the stack into the valueOut parameter. 
// Returns true if successful, false otherwise.

bool CS_isEmpty(CharStack *cs);
// Returns true if the stack is empty, false otherwise.

bool CS_isFull(CharStack *cs);
// Returns true if the stack is full, false otherwise.

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

bool CS_addValue(CharStack *cs, CharStackValue value) {
    if (CS_isFull(cs)) {
        // Stack is full, cannot add more values
        return false;
    }
    cs->stack[cs->head] = value;                // Add value to the head
    cs->head++;                                 // Increment head
    return true;
}

bool CS_addStringValue(CharStack *cs, const unsigned char *str) {
    while (*str != '\0') { // Continue until we reach the null terminator
        if (!CS_addValue(cs, *str)) {
            // If the stack gets full, return false
            return false;
        }
        str++; // Move to the next character in the string
    }
    return true; // successfully added all characters
}

CharStackValue CS_popValue(CharStack *cs) {
    if (CS_isEmpty(cs)) {
        return 0;
    }
    CharStackValue out = cs->stack[cs->head - 1];   // Get value from before the head
    cs->head--;                                    // Decrement head
    return out;
}

bool CS_popValueSAFE(CharStack *cs, CharStackValue *valueOut) {
    if (CS_isEmpty(cs)) {
        // Stack is empty, cannot pop any values
        return false;
    }
    *valueOut = CS_popValue(cs);   // Pop value
    return true;
}

bool CS_isEmpty(CharStack *cs) {
    return cs->head == 0;
}

bool CS_isFull(CharStack *cs) {
    return cs->head >= CHARSTACK_SIZE;
}











// Inclusion guard
#endif  // FSWCHARSTACK_INCLUDED