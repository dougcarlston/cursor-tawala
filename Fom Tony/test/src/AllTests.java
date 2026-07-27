import junit.framework.Test;
import junit.framework.TestSuite;

import com.scissor.TestSuiteBuilder;
import com.tawala.TestCase;

public class AllTests extends TestCase {

    public static Test suite() throws ClassNotFoundException {
        String fullPath = System.getProperty("com.scissor.TestSuiteBuilder.path", "./");

        TestSuiteBuilder builder = new TestSuiteBuilder(fullPath + "test/src");
        TestSuite result = builder.build("com.tawala.acceptance");
        
        builder = new TestSuiteBuilder(fullPath + "test/src");
        builder.skipPackage("com.tawala.acceptance");

        result.addTest(builder.build("com.tawala", "com.scissor"));
        
        return result;
    }
}
