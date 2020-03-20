using System;
using System.Collections.Generic;
using System.Text;

namespace CalveryApexBot
{
    public partial class ApexApiResponse
    {
        public Data Data { get; set; }
    }

    public partial class Data
    {
        public PlatformInfo PlatformInfo { get; set; }
        public UserInfo UserInfo { get; set; }
        public DataMetadata Metadata { get; set; }
        public Segment[] Segments { get; set; }
        public AvailableSegment[] AvailableSegments { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }

    public partial class AvailableSegment
    {
        public string Type { get; set; }
        public MetadataClass Attributes { get; set; }
    }

    public partial class MetadataClass
    {
    }

    public partial class DataMetadata
    {
        public long? CurrentSeason { get; set; }
        public string ActiveLegend { get; set; }
        public string ActiveLegendName { get; set; }
    }

    public partial class PlatformInfo
    {
        public string PlatformSlug { get; set; }
        public string PlatformUserId { get; set; }
        public string PlatformUserHandle { get; set; }
        public string PlatformUserIdentifier { get; set; }
        public Uri AvatarUrl { get; set; }
        public object AdditionalParameters { get; set; }
    }

    public partial class Segment
    {
        public string Type { get; set; }
        public SegmentAttributes Attributes { get; set; }
        public SegmentMetadata Metadata { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public Stats Stats { get; set; }
    }

    public partial class SegmentAttributes
    {
        public string Id { get; set; }
    }

    public partial class SegmentMetadata
    {
        public string Name { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri TallImageUrl { get; set; }
        public Uri BgImageUrl { get; set; }
        public bool? IsActive { get; set; }
    }

    public partial class Stats
    {
        public Bamboozles Level { get; set; }
        public Bamboozles Kills { get; set; }
        public Bamboozles KillsAsKillLeader { get; set; }
        public Bamboozles Damage { get; set; }
        public Bamboozles Headshots { get; set; }
        public Bamboozles Finishers { get; set; }
        public Bamboozles SniperKills { get; set; }
        public RankScore RankScore { get; set; }
        public Bamboozles Season3Wins { get; set; }
        public Bamboozles Season4Wins { get; set; }
        public Bamboozles DroppodItemsForSquadmates { get; set; }
        public Bamboozles Bamboozles { get; set; }
        public Bamboozles DecoysCreated { get; set; }
        public Bamboozles FriendlyShieldsCharged { get; set; }
    }

    public partial class Bamboozles
    {
        public long? Rank { get; set; }
        public double? Percentile { get; set; }
        public string DisplayName { get; set; }
        public DisplayCategory? DisplayCategory { get; set; }
        public object Category { get; set; }
        public MetadataClass Metadata { get; set; }
        public long? Value { get; set; }
        public string DisplayValue { get; set; }
        public DisplayType? DisplayType { get; set; }
    }

    public partial class RankScore
    {
        public object Rank { get; set; }
        public double? Percentile { get; set; }
        public string DisplayName { get; set; }
        public DisplayCategory? DisplayCategory { get; set; }
        public object Category { get; set; }
        public RankScoreMetadata Metadata { get; set; }
        public long? Value { get; set; }
        public string DisplayValue { get; set; }
        public DisplayType? DisplayType { get; set; }
    }

    public partial class RankScoreMetadata
    {
        public Uri IconUrl { get; set; }
    }

    public partial class UserInfo
    {
        public bool? IsPremium { get; set; }
        public bool? IsVerified { get; set; }
        public bool? IsInfluencer { get; set; }
        public object CountryCode { get; set; }
        public object CustomAvatarUrl { get; set; }
        public object[] SocialAccounts { get; set; }
    }

    public enum DisplayCategory { Combat, Game, Weapons };

    public enum DisplayType { Unspecified };

}
