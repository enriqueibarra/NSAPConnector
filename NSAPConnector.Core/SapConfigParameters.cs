using System;

namespace NSAPConnector
{
    /// <summary>
    /// This class contains all possible configuration parameters names
    /// for a SAP configuration.
    /// </summary>
    public static class SapConfigParameters
    {
        public const string AppServerHost = "ASHOST";
        public const string AppServerService = "ASSERV";
        public const string MessageServerHost = "MSHOST";
        public const string MessageServerService = "MSSERV";
        public const string LogonGroup = "GROUP";
        public const string GatewayHost = "GWHOST";
        public const string GatewayService = "GWSERV";
        public const string SystemNumber = "SYSNR";
        public const string User = "USER";
        public const string AliasUser = "ALIAS_USER";
        public const string Password = "PASSWD";
        public const string Client = "CLIENT";
        public const string Language = "LANG";
        public const string Codepage = "CODEPAGE";
        public const string PartnerCharSize = "PCS";
        public const string SystemID = "SYSID";
        public const string SystemIDs = "SYS_IDS";
        public const string X509Certificate = "X509CERT";
        public const string SAPSSO2Ticket = "MYSAPSSO2";
        public const string ExternalIDData = "EXTIDDATA";
        public const string ExternalIDType = "EXTIDTYPE";
        public const string UseSAPGui = "USE_SAPGUI";
        public const string AbapDebug = "ABAP_DEBUG";
        public const string LogonCheck = "LCHECK";
        public const string ProgramID = "PROGRAM_ID";
        public const string SncMode = "SNC_MODE";
        public const string SncMyName = "SNC_MYNAME";
        public const string SncPartnerName = "SNC_PARTNERNAME";
        public const string SncPartnerNames = "SNC_PARTNER_NAMES";
        public const string SncLibraryPath = "SNC_LIB";
        public const string SncQOP = "SNC_QOP";
        public const string SncSSO = "SNC_SSO";
        public const string Trace = "TRACE";
        public const string SAPRouter = "SAPROUTER";
        public const string NoCompression = "NO_COMPRESSION";
        public const string Delta = "DELTA";
        public const string OnCharacterConversionError = "ON_CCE";
        public const string CharacterFaultIndicatorToken = "CFIT";
        public const string PeakConnectionsLimit = "MAX_POOL_SIZE";
        [Obsolete("use PeakConnectionsLimit instead")]
        public const string MaxPoolSize = "MAX_POOL_SIZE";
        public const string PoolSize = "POOL_SIZE";
        public const string IdleTimeout = "IDLE_TIMEOUT";
        public const string IdleCheckTime = "IDLE_CHECK_TIME";
        public const string MaxPoolWaitTime = "MAX_POOL_WAIT_TIME";
        public const string MaxShutdownWaitTime = "MAX_SHUTDOWN_WAIT_TIME";
        public const string RegistrationCount = "REG_COUNT";
        public const string MaxRegistrationCount = "MAX_REG_COUNT";
        public const string ServerRestartTimeout = "SERVER_RESTART_TIMEOUT";
        public const string PasswordChangeEnforced = "PASSWORD_CHANGE_ENFORCED";
        public const string Name = "NAME";
        public const string RepositoryDestination = "REPOSITORY_DESTINATION";
        public const string RepositoryUser = "REPOSITORY_USER";
        public const string RepositoryPassword = "REPOSITORY_PASSWD";
        public const string RepositorySncMyName = "REPOSITORY_SNC_MYNAME";
        public const string RepositoryX509Certificate = "REPOSITORY_X509CERT";
        public const string UseSAPCodepages = "USE_SAP_CODEPAGES";
    }
}
