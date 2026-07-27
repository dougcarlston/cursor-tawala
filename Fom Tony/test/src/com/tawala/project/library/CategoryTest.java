package com.tawala.project.library;

import java.util.ArrayList;
import java.util.List;

import junit.framework.TestCase;

public class CategoryTest extends TestCase {
	@Override
	protected void setUp() throws Exception {
		super.setUp();
		ProjectLibraryTestCase.setupLuceneForUnitTest();
	}
    
    public void testBasics() {
        Category a = new Category(ProjectLibrary.COMMUNITY_LIBRARY, "a", "description of a");
        Category b = new Category(a, "b", "description of b");
        
        assertEquals(1, a.getSubcategories().size());
        assertEquals(b, a.getSubcategories().iterator().next());
        
        assertFalse(a.isLeaf());
        assertTrue(b.isLeaf());
        
        assertNotSame(a, b);
        assertSame(a, a);
        assertSame(b, b);
    }
    
    public void testMovingNodes() {
        Category a = new Category(ProjectLibrary.COMMUNITY_LIBRARY, "a", "description of a");
        Category b = new Category(a, "b", "description of b");
        Category c = new Category(ProjectLibrary.COMMUNITY_LIBRARY, "c", "description of c");

        b.setParent(c);
        
        assertEquals(0, a.getSubcategories().size());
        assertTrue(a.isLeaf());
        
        assertEquals(1, c.getSubcategories().size());
        assertEquals(b, c.getSubcategories().iterator().next());
        assertFalse(c.isLeaf());
        
        assertTrue(b.isLeaf());
        assertEquals(c, b.getParent());
        
        try {
            a.setParent(a);
            fail("Should have complained about circular tree");
        } catch (IllegalArgumentException e) {
            // TODO: handle exception
        }

        try {
            c.setParent(b);
            fail("Should have complained about circular tree");
        } catch (IllegalArgumentException e) {
            // TODO: handle exception
        }

    }

    public void testFlattening() {
        Category a = new Category(ProjectLibrary.COMMUNITY_LIBRARY, "a", "description of a");
        Category b = new Category(a, "b", "description of b");
        Category c = new Category(b, "c", "description of c");
        Category d = new Category(b, "d", "description of c");
        Category e = new Category(d, "e", "description of c");
        Category f = new Category(c, "f", "description of c");
        
        List<Category> flattenedTree = new ArrayList<Category>(); 
        a.flattenOnto(flattenedTree);

        assertEquals(6, flattenedTree.size());
        assertEquals(a, flattenedTree.get(0));
        assertEquals(b, flattenedTree.get(1));
        assertEquals(c, flattenedTree.get(2));
        assertEquals(f, flattenedTree.get(3));
        assertEquals(d, flattenedTree.get(4));
        assertEquals(e, flattenedTree.get(5));
    }

    public void testRetrievingAllParents() {
        Category a = new Category(ProjectLibrary.COMMUNITY_LIBRARY, "a", "description of a");
        Category b = new Category(a, "b", "description of b");
        Category d = new Category(b, "d", "description of c");
        Category e = new Category(d, "e", "description of c");
        
        List<Category> parents = a.getAllParents();
        assertEquals(0, parents.size());

        parents = b.getAllParents();
        assertEquals(1, parents.size());
        assertEquals(a, parents.get(0));

        parents = e.getAllParents();
        assertEquals(3, parents.size());
        assertEquals(a, parents.get(0));
        assertEquals(b, parents.get(1));
        assertEquals(d, parents.get(2));
        
        assertTrue(e.isChildOf(a));
        assertTrue(e.isChildOf(b));
        assertTrue(e.isChildOf(d));
        assertFalse(a.isChildOf(a));
        assertFalse(a.isChildOf(b));
    }
    
    public void testGetDepth() {
    	Category topLevel = new Category(ProjectLibrary.COMMUNITY_LIBRARY, "top level", "Description");
    	Category secondLevel = new Category(topLevel, "second level", "Description");
    	Category thirdLevel = new Category(secondLevel, "third level", "Description");
    	
    	assertEquals(0, topLevel.getDepth());
    	assertEquals(1, secondLevel.getDepth());
    	assertEquals(2, thirdLevel.getDepth());
    }
}
