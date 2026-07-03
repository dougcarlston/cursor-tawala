package mock.javax.servlet.http;


import java.util.Arrays;
import java.util.HashSet;
import java.util.Set;

import com.tawala.TestCase;

public class FakeServletContextTest extends TestCase {

    public void testGetResourcePathsFiles() {
        FakeServletContext context = new FakeServletContext();
        context.setResource("/apple.txt", new byte[0]);
        context.setResource("/banana.txt", new byte[0]);
        assertEquals(makeSet("/apple.txt", "/banana.txt"), context.getResourcePaths("/"));
    }

    public void testGetResourcePathsWithDirectory() {
        FakeServletContext context = new FakeServletContext();
        context.setResource("/apple/granny-smith.txt", new byte[0]);
        context.setResource("/apple/red/delicious.txt", new byte[0]);
        context.setResource("/banana.txt", new byte[0]);
        assertEquals(makeSet("/apple/", "/banana.txt"), context.getResourcePaths("/"));
    }

    public void testSubdirectory() {
        FakeServletContext context = new FakeServletContext();
        context.setResource("/apple/granny-smith.txt", new byte[0]);
        context.setResource("/apple/red/delicious.txt", new byte[0]);
        context.setResource("/banana.txt", new byte[0]);
        assertEquals(makeSet("/apple/granny-smith.txt", "/apple/red/"), context.getResourcePaths("/apple/"));
    }

    public void testNoResources() {
        FakeServletContext context = new FakeServletContext();
        assertNull(context.getResourcePaths("/"));
    }

    // trailing "/" on subdirectory missing?

    private Set makeSet(String firstElements, String secondElement) {
        return new HashSet<String>(Arrays.asList(new String[]{firstElements, secondElement}));
    }

}
