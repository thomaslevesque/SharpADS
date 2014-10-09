SharpADS
========

A library that provides access to NTFS alternate data streams.

The library has just two classes:

- `AdsFile` exposes static methods similar to the ones in `System.IO.File`, except that you specify the stream name in addition to the path:

```csharp
AdsFile.WriteAllText("foobar.txt", "secret", "hello world");
```


- `AdsFileStream` is the equivalent of `System.IO.FileStream` (and actually inherits from it) for alternate data streams. It can be used exactly like `FileStream`, except that the constructor takes the stream name in addition to the path:

```csharp
using (var stream = new AdsFileStream("foobar.txt", "secret", FileMode.Create, FileAccess.Write))
{
    ...
}
```
