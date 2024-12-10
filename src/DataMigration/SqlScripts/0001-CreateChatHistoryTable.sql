CREATE TABLE chathistory (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    creatorId INT NULL,
    createdOn TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    lastModifiedOn TIMESTAMPTZ NULL,
    message JSONB NULL
);

-- Add a B-tree index on the 'message' column
CREATE INDEX idx_chathistory_message ON chathistory USING BTREE (message);
