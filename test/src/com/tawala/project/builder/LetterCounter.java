package com.tawala.project.builder;

import com.scissor.UnimplementedException;

public class LetterCounter {
    private char letter;

    private LetterCounter(char start) {
        this.letter = start;
    }

    public LetterCounter() {
        this('a');
    }

    public String next() {
        if (letter > 'z') throw new UnimplementedException();
        return "" + letter++;
    }
}
