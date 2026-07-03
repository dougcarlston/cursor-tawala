package com.scissor;

import com.tawala.TestCase;

@ExcludeFromTests    
public class StopwatchTest extends TestCase {
    private Stopwatch watch;

    protected void setUp() throws Exception {
        super.setUp();
        watch = new Stopwatch();
    }

    public void testInitialState() {
        assertEquals(0, watch.time());
    }

    public void testNormalBehavior() {
        watch.start();

        sleep(10);
        long time = watch.time();
        assertTrue(time >= 10);

        sleep(10);
        time = watch.time();
        assertTrue(time >= 20);

        time = watch.time();
        watch.stop();
        assertEquals(time, watch.time());
        sleep(10);
        assertEquals(time, watch.time());
    }

    public void testReset() {
        watch.start();
        sleep(10);
        watch.stop();
        long time = watch.time();
        assertTrue(time >= 10);

        watch.reset();
        assertEquals(0, watch.time());
    }

    private void sleep(int ms) {
        try {
            Thread.sleep(ms);
        } catch (InterruptedException e) {
            throw new RuntimeException("unexpected interruption", e);
        }
    }

}
