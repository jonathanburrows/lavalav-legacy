## Goals
To provide a way to convert a stream to an entity.

## Requirements
Must provide a generic method, Deserialize, which accepts a Stream and:
1. Converts the stream to the given type
2. If the stream cannot be deserialized to the type, throws an InvalidOperationException
3. If the stream is null, throws an ArgumentNullException