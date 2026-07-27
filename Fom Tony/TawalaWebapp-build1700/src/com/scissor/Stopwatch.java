package com.scissor;

public class Stopwatch {
    long totalTime;
    long start;

    public void start() {
        start = now();
    }

    public void stop() {
        totalTime += currentRunTime();
        start = 0;
    }

    public long time() {
        return totalTime + currentRunTime();
    }

    private long currentRunTime() {
        if (start == 0) return 0;
        return now() - start;
    }

    private long now() {
        return System.currentTimeMillis();
    }

    public void reset() {
        stop();
        totalTime = 0;
    }
}
