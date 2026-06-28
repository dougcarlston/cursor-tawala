-- Fix dev users seeded with invalid status 'APPROVED' (Java enum is REGISTERED).
UPDATE users SET status = 'REGISTERED' WHERE user_name IN ('dev', 'designer') AND status = 'APPROVED';
