namespace Ares.AudioSource.Freesound
{
    /// <summary>
    /// This class holds the Freesound API key.
    /// 
    /// The Freesound API Terms of Service do not allow including an API key in the released sources.
    /// So if you want to compile the Freesound AudioSource plugin for ARES please apply for your own
    /// API key at https://www.freesound.org/apiv2/apply/ and include it in the ApiKey class.
    /// Don't forge to rename the ApiKey.example.cs to ApiKey.cs as well!
    /// 
    /// Note: Distributing a compiled binary which includes an API key is allowed.
    /// This has been confirmed on the Freesound API Google Group:
    ///      https://groups.google.com/forum/#!topic/freesound-api/Ott9TX7bty4
    /// 
    /// </summary>
    class ApiKey
    {
        public const string Key = "YOUR_KEY_HERE";
    }
}
