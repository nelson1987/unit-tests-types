CREATE TABLE IF NOT EXISTS customers (
  id UUID PRIMARY KEY,
  name VARCHAR(150) NOT NULL,
  email VARCHAR(150) NOT NULL,
  document VARCHAR(50) NOT NULL,
  created_at TIMESTAMP NOT NULL
);

CREATE OR REPLACE PROCEDURE get_customer_by_id_proc(IN p_id UUID, OUT p_result TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
  SELECT json_build_object(
    'id', id,
    'name', name,
    'email', email,
    'document', document,
    'createdAt', created_at
  )::text
  INTO p_result
  FROM customers
  WHERE id = p_id;
END;
$$;
