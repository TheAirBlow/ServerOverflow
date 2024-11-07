using System.Text.Json;
using MongoDB.Bson.Serialization.Attributes;
using Serilog;

namespace ServerOverflow.Database;

/// <summary>
/// A minecraft server
/// </summary>
public class Server : Document {
    /// <summary>
    /// Server's IP address
    /// </summary>
    [BsonElement("ip")] 
    public string IP { get; set; } = "";
    
    /// <summary>
    /// Server's port
    /// </summary>
    public int Port { get; set; }
    
    /// <summary>
    /// When was the server last seen
    /// </summary>
    [BsonElement("timestamp")]
    public DateTime LastSeen { get; set; }
    
    /// <summary>
    /// When was the server last empty
    /// </summary>
    public DateTime? LastEmpty { get; set; }
    
    /// <summary>
    /// When was the server last active
    /// </summary>
    public DateTime? LastActive { get; set; }

    /// <summary>
    /// Server list ping object
    /// </summary>
    [BsonElement("minecraft")]
    public ServerListPing Ping { get; set; } = new();

    /// <summary>
    /// Online mode guess
    /// </summary>
    public OnlineMode OnlineModeGuess { get; set; }
    
    /// <summary>
    /// Server's fingerprint
    /// </summary>
    public Fingerprint? Fingerprint { get; set; }
    
    /// <summary>
    /// Dictionary of seen players
    /// </summary>
    public Dictionary<string, PlayerInfo>? Players { get; set; }
    
    /// <summary>
    /// Fetches a server by its identifier
    /// </summary>
    /// <param name="id">Object identifier</param>
    /// <returns>Server, may be null</returns>
    public static async Task<Server?> Get(string id)
        => await Controller.Servers.QueryFirst(x => x.Id.ToString() == id);
}

/// <summary>
/// Online mode guess
/// </summary>
public enum OnlineMode {
    Offline = 0,
    Online = 1,
    Mixed = 2
}

/// <summary>
/// Player information
/// </summary>
public class PlayerInfo {
    /// <summary>
    /// When was the player last seen
    /// </summary>
    public DateTime LastSeen { get; set; }
    
    /// <summary>
    /// Player's username
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// Fingerprint class
/// </summary>
public class Fingerprint {
    /// <summary>
    /// Passive fingerprint
    /// </summary>
    public PassiveFingerprint? Passive { get; set; }
    
    /// <summary>
    /// Passive fingerprint
    /// </summary>
    public ActiveFingerprint? Active { get; set; }
}

/// <summary>
/// Active fingerprint
/// </summary>
public class PassiveFingerprint {
    /// <summary>
    /// Was the favicon empty
    /// </summary>
    public bool EmptyFavicon { get; set; }
    
    /// <summary>
    /// Was the player sample empty
    /// </summary>
    public bool EmptySample { get; set; }
    
    /// <summary>
    /// Was the SLP field order incorrect
    /// </summary>
    public bool IncorrectOrder { get; set; }
    
    /// <summary>
    /// The incorrect order of fields
    /// </summary>
    public string? FieldOrder { get; set; }
}

/// <summary>
/// Active fingerprint
/// </summary>
public class ActiveFingerprint {
    /// <summary>
    /// When was active fingerprinting performed
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Server software identified
    /// </summary>
    public string Software { get; set; } = "";
}

/// <summary>
/// Server list ping object
/// </summary>
public class ServerListPing {
    /// <summary>
    /// Version information
    /// </summary>
    public class VersionClass {
        /// <summary>
        /// Human-readable version string
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Protocol version
        /// </summary>
        public int? Protocol { get; set; }
    }

    /// <summary>
    /// Player sample and other
    /// </summary>
    public class PlayersClass {
        /// <summary>
        /// Player sample
        /// </summary>
        public class SampleClass {
            /// <summary>
            /// Player's username
            /// </summary>
            public string? Name { get; set; }
            
            /// <summary>
            /// Player's UUID
            /// </summary>
            [BsonElement("id")]
            public string? UUID { get; set; }
        }
        
        /// <summary>
        /// Online players
        /// </summary>
        public int? Online { get; set; }
        
        /// <summary>
        /// Maximum players
        /// </summary>
        public int? Max { get; set; }
        
        /// <summary>
        /// Player samples
        /// </summary>
        public List<SampleClass>? Sample { get; set; } = [];
    }

    /// <summary>
    /// Legacy forge mod info
    /// </summary>
    public class LegacyModInfo {
        /// <summary>
        /// Mod information
        /// </summary>
        public class ModClass {
            /// <summary>
            /// Mod's identifier
            /// </summary>
            [BsonElement("modid")]
            public string? ModId { get; set; }
            
            /// <summary>
            /// Mod's version
            /// </summary>
            public string? Version { get; set; }
        }

        /// <summary>
        /// List of mods
        /// </summary>
        public List<ModClass>? ModList { get; set; } = [];
    }
        
    /// <summary>
    /// Modern forge info
    /// </summary>
    public class ModernModInfo {
        /// <summary>
        /// Mod information
        /// </summary>
        public class ModClass {
            /// <summary>
            /// Mod's identifier
            /// </summary>
            public string? ModId { get; set; }
                
            /// <summary>
            /// Mod's version
            /// </summary>
            [BsonElement("modmarker")] 
            public string? Version { get; set; }
        }

        /// <summary>
        /// List of mods
        /// </summary>
        [BsonElement("mods")]
        public List<ModClass>? ModList { get; set; } = [];
    }
    
    /// <summary>
    /// Modern forge modlist
    /// </summary>
    [BsonElement("forgeData")]
    public ModernModInfo? ModernForgeMods { get; set; }

    /// <summary>
    /// Legacy forge modlist
    /// </summary>
    [BsonElement("modinfo")] 
    public LegacyModInfo? LegacyForgeMods { get; set; }
    
    /// <summary>
    /// Version information
    /// </summary>
    public VersionClass? Version { get; set; }
    
    /// <summary>
    /// Players information
    /// </summary>
    public PlayersClass? Players { get; set; }
    
    /// <summary>
    /// Server's description JSON
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Server's description, cleaned of all decorations
    /// </summary>
    public string? CleanDescription { get; set; }
    
    /// <summary>
    /// Server's favicon
    /// </summary>
    public string? Favicon { get; set; }
    
    /// <summary>
    /// Is chat reporting enabled
    /// </summary>
    [BsonElement("enforcesSecureChat")] 
    public bool ChatReporting { get; set; }
    
    /// <summary>
    /// Is this a forge server
    /// </summary>
    public bool IsForge { get; set; }

    /// <summary>
    /// Encodes the description into HTML
    /// </summary>
    /// <returns>Raw HTML</returns>
    public string? DescriptionToHtml() {
        if (Description == null) return null;
        try {
            var str = JsonSerializer.Deserialize<string>(Description);
            if (str != null) return ColorEncoding.ToHtml(str);
        } catch { /* Ignore */ }

        try {
            var obj = JsonSerializer.Deserialize<ChatComponent>(Description);
            return obj?.ToHtml();
        } catch (Exception e) {
            return "<b>Failed to deserialize the chat component!</b>";
        }
    }
}