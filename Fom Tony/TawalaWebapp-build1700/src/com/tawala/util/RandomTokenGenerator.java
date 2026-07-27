package com.tawala.util;

import java.util.Random;

public class RandomTokenGenerator {
    private static final Random random = new Random(System.currentTimeMillis());

    /**
     * Characters that are going to be used to generate temporary password.
     */
    private static final char[] DEFAULT_CHARACTERS_IN_TOKEN = new char[] { 'a', 'b',
            'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
            'p', 'r', 'q', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', /* 'A', 'B',
            'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
            'P', 'R', 'Q', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', */ '1', '2',
            '3', '4', '5', '6', '7', '8', '9', '0' };

	public static String getRandomToken(int length) {
        byte[] randomBytes = new byte[length];

        random.nextBytes(randomBytes);

        StringBuffer result = new StringBuffer();

        for (int i = 0; i < randomBytes.length; i++) {
            byte b = randomBytes[i];
            char nextChar = DEFAULT_CHARACTERS_IN_TOKEN[Math.abs(b)
                    % DEFAULT_CHARACTERS_IN_TOKEN.length];
            result.append(nextChar);
        }

        return result.toString();
	}
}
