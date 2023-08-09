using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KZLib.Auth
{
    /// <summary>
    /// A list of Team Drives.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class GoogleTeamDriveList : IResourceData
    {
        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#teamDriveList".
        /// </summary>
        public string Kind => "drive#teamDriveList";
        /// <summary>
        /// The page token for the next page of Team Drives. This will be absent if the end
        /// of the Team Drives list has been reached. If the token is rejected for any reason,
        /// it should be discarded, and pagination should be restarted from the first page of results.
        /// </summary>
        public string NextPageToken { get; private set; }
        /// <summary>
        /// The list of Team Drives. If nextPageToken is populated, then this list may be
        /// incomplete and an additional page of results should be fetched.
        /// </summary>
        public List<GoogleTeamDrive> TeamDrives { get; private set; }
    }
}
