package au.com.fullcirclesolutions.internaltesting;

import static org.junit.Assert.assertEquals;

import org.junit.Test;

public class TestJUnit {

    @Test
    public void testAdd() {
        String str = "Junit is working fine";
        assertEquals("Junit is working fine", str);
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 15th November 2013
 */
