using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AccessGrid
{
    public class Device
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }

        [JsonPropertyName("device_type")]
        public DeviceType DeviceType { get; set; }

        [JsonPropertyName("status")]
        public DeviceStatus Status { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class AccessCard
    {
        [JsonConstructor]
        internal AccessCard(string id, string url, AccessPassState? state)
        {
            Id = id;
            Url = url;
            State = state;
        }

        public AccessCard()
        {
        }

        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonPropertyName("install_url")]
        public string Url { get; private set; }

        [JsonPropertyName("state")]
        public AccessPassState? State { get; private set; }

        /// <summary>
        /// Unique identifier for the card template to use
        /// </summary>
        [JsonPropertyName("card_template_id")]
        public string CardTemplateId { get; set; }

        /// <summary>
        /// Unique identifier for the employee
        /// </summary>
        [JsonPropertyName("employee_id")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// Physical tag identifier associated with the NFC key, only allowed if your card template has key diversification enabled
        /// </summary>
        [JsonPropertyName("tag_id")]
        public string TagId { get; set; }

        /// <summary>
        /// Site code from H10301 (26 bit) format. Must be number under 255
        /// </summary>
        [JsonPropertyName("site_code")]
        public string SiteCode { get; set; }

        /// <summary>
        /// Site code from H10301 (26 bit) format. Must be number under 65,535
        /// </summary>
        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; }

        /// <summary>
        /// Optional credential pool ID to use for automatic credential assignment. When provided, the system will automatically assign site_code and card_number from the pool.
        /// </summary>
        [JsonPropertyName("credential_pool_id")]
        public string CredentialPoolId { get; set; }

        /// <summary>
        /// Up to 8192 bytes of data. Only used when using your own proprietary data format
        /// </summary>
        [JsonPropertyName("file_data")]
        public string FileData { get; set; }

        /// <summary>
        /// Full name of the employee
        /// </summary>
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        /// <summary>
        /// Email address of the employee - if you have a SendGrid or Postmark integration, then we will send a text message on your behalf
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// Contact phone number for the employee - if you have a Twilio integration, then we will send a text message on your behalf
        /// </summary>
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Employment classification (e.g., full_time, contractor)
        /// </summary>
        [JsonPropertyName("classification")]
        public string Classification { get; set; }

        /// <summary>
        /// Employee title or role within the organization
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Name of the organization the employee belongs to
        /// </summary>
        [JsonPropertyName("organization_name")]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Department within the organization
        /// </summary>
        [JsonPropertyName("department")]
        public string Department { get; set; }

        /// <summary>
        /// Location or office name
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// Site name (e.g., building or campus)
        /// </summary>
        [JsonPropertyName("site_name")]
        public string SiteName { get; set; }

        /// <summary>
        /// Workstation identifier
        /// </summary>
        [JsonPropertyName("workstation")]
        public string Workstation { get; set; }

        /// <summary>
        /// Mail stop code
        /// </summary>
        [JsonPropertyName("mail_stop")]
        public string MailStop { get; set; }

        /// <summary>
        /// Company address
        /// </summary>
        [JsonPropertyName("company_address")]
        public string CompanyAddress { get; set; }

        /// <summary>
        /// ISO8601 timestamp when the card becomes active
        /// </summary>
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// ISO8601 timestamp when the card expires
        /// </summary>
        [JsonPropertyName("expiration_date")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Whether this is a temporary pass (DESFire and SmartTap only, max 24h)
        /// </summary>
        [JsonPropertyName("temporary")]
        public bool? Temporary { get; set; }

        /// <summary>
        /// Base64 encoded image of the employee
        /// </summary>
        [JsonPropertyName("employee_photo")]
        public string EmployeePhoto { get; set; }

        /// <summary>
        /// Only for 'hotel' use case. Loyalty/membership ID of the guest
        /// </summary>
        [JsonPropertyName("member_id")]
        public string MemberId { get; set; }

        /// <summary>
        /// Only for 'hotel' use case. Status of the guest's membership
        /// </summary>
        [JsonPropertyName("membership_status")]
        public string MembershipStatus { get; set; }

        /// <summary>
        /// Only for 'hotel' use case. Whether the pass is ready to be used for transactions
        /// </summary>
        [JsonPropertyName("is_pass_ready_to_transact")]
        public bool? IsPassReadyToTransact { get; set; }

        /// <summary>
        /// Only for 'hotel' use case. Data for the hotel tile display
        /// </summary>
        [JsonPropertyName("tile_data")]
        public object TileData { get; set; }

        /// <summary>
        /// Only for 'hotel' use case. Data for the hotel reservation
        /// </summary>
        [JsonPropertyName("reservations")]
        public object Reservations { get; set; }

        /// <summary>
        /// False by default. Set to true if you'd like to enable the NFC keys issued using this template to exist on multiple devices
        /// </summary>
        [JsonPropertyName("allow_on_multiple_devices")]
        public bool? AllowOnMultipleDevices { get; private set; }

        [JsonPropertyName("details")]
        public IReadOnlyList<AccessCard> Details { get; set; }

        [JsonPropertyName("direct_install_url")]
        public string DirectInstallUrl { get; set; }

        [JsonPropertyName("devices")]
        public List<Device> Devices { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        public override string ToString()
        {
            return $"AccessCard(name='{FullName}', id='{Id}', state='{State}')";
        }
    }

    public class Template
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("use_case")]
        public string UseCase { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("last_published_at")]
        public string LastPublishedAt { get; set; }

        [JsonPropertyName("issued_keys_count")]
        public int? IssuedKeysCount { get; set; }

        [JsonPropertyName("active_keys_count")]
        public int? ActiveKeysCount { get; set; }

        [JsonPropertyName("allowed_device_counts")]
        public object AllowedDeviceCounts { get; set; }

        [JsonPropertyName("support_settings")]
        public object SupportSettings { get; set; }

        [JsonPropertyName("terms_settings")]
        public object TermsSettings { get; set; }

        [JsonPropertyName("style_settings")]
        public object StyleSettings { get; set; }
    }

    public class ListKeysRequest
    {
        /// <summary>
        /// Required. The card template ID to list keys for
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Filter keys by state (active, suspended, unlink, deleted)
        /// </summary>
        public string State { get; set; }
    }

    public class ProvisionCardRequest : AccessCard
    {
        [JsonConstructor]
        internal ProvisionCardRequest(string id, string url, AccessPassState state) : base(id, url,
            state)
        {
        }

        public ProvisionCardRequest() : base()
        {
        }
    }

    public class UpdateCardRequest : AccessCard
    {
        [JsonConstructor]
        internal UpdateCardRequest(string id, string url, AccessPassState state) : base(id, url,
            state)
        {
        }

        public UpdateCardRequest() : base()
        {
        }
    }

    public class TemplateDesign
    {
        /// <summary>
        /// Must be a 6 character hexadecimal value for the background color of the template, i.e. #FFFFFF
        /// </summary>
        [JsonPropertyName("background_color")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Must be a 6 character hexadecimal value for the label color for the template, i.e. #000000
        /// </summary>
        [JsonPropertyName("label_color")]
        public string LabelColor { get; set; }

        /// <summary>
        /// Must be a 6 character hexadecimal value for the secondary label color for the template, i.e. #333333
        /// </summary>
        [JsonPropertyName("label_secondary_color")]
        public string LabelSecondaryColor { get; set; }

        /// <summary>
        /// Base64 encoded image of the card templates background
        /// </summary>
        [JsonPropertyName("background_image")]
        public string BackgroundImage { get; set; }

        /// <summary>
        /// Base64 encoded image of the card templates logo (located in the top left)
        /// </summary>
        [JsonPropertyName("logo_image")]
        public string LogoImage { get; set; }

        /// <summary>
        /// Base64 encoded image of the card templates icon (used in sharing and notifications)
        /// </summary>
        [JsonPropertyName("icon_image")]
        public string IconImage { get; set; }
    }

    public class SupportInfo
    {
        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_url")]
        public string SupportUrl { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_phone_number")]
        public string SupportPhoneNumber { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_email")]
        public string SupportEmail { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("privacy_policy_url")]
        public string PrivacyPolicyUrl { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("terms_and_conditions_url")]
        public string TermsAndConditionsUrl { get; set; }
    }

    public class CreateTemplateRequest
    {
        /// <summary>
        /// The name to display for this card template in the AccessGrid console UI
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Must be one of `apple` or `google`
        /// </summary>
        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }

        /// <summary>
        /// Must be `employee_badge`
        /// </summary>
        [JsonPropertyName("use_case")]
        public string UseCase { get; set; }

        /// <summary>
        /// Must be `desfire` or `seos` - HID Seos only available for enterprise customers
        /// </summary>
        [JsonPropertyName("protocol")]
        public Protocol Protocol { get; set; }

        /// <summary>
        /// False by default. Set to true if you'd like to enable the NFC keys issued using this template to exist on multiple devices (think phone and watch)
        /// </summary>
        [JsonPropertyName("allow_on_multiple_devices")]
        public bool? AllowOnMultipleDevices { get; set; }

        /// <summary>
        /// Only allowed to be set if `allow_on_multiple_devices` is set to true. Any number between 1-5 is acceptable.
        /// </summary>
        [JsonPropertyName("watch_count")]
        public int? WatchCount { get; set; }

        /// <summary>
        /// Only allowed to be set if `allow_on_multiple_devices` is set to true. Any number between 1-5 is acceptable.
        /// </summary>
        [JsonPropertyName("iphone_count")]
        public int? IPhoneCount { get; set; }

        /// <summary>
        /// Must be a 6 character hexadecimal value for the background color, i.e. #FFFFFF
        /// </summary>
        [JsonPropertyName("background_color")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Must be a 6 character hexadecimal value for the label color, i.e. #000000
        /// </summary>
        [JsonPropertyName("label_color")]
        public string LabelColor { get; set; }

        /// <summary>
        /// Must be a 6 character hexadecimal value for the secondary label color, i.e. #333333
        /// </summary>
        [JsonPropertyName("label_secondary_color")]
        public string LabelSecondaryColor { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_url")]
        public string SupportUrl { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_phone_number")]
        public string SupportPhoneNumber { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_email")]
        public string SupportEmail { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("privacy_policy_url")]
        public string PrivacyPolicyUrl { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("terms_and_conditions_url")]
        public string TermsAndConditionsUrl { get; set; }

        /// <summary>
        /// Optional metadata key-value pairs
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class UpdateTemplateRequest
    {
        /// <summary>
        /// The card template id you want to update
        /// </summary>
        [JsonPropertyName("card_template_id")]
        public string CardTemplateId { get; set; }

        /// <summary>
        /// The name to display for this card template in the AccessGrid console UI
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// False by default. Set to true if you'd like to enable the NFC keys issued using this template to exist on multiple devices (think phone and watch)
        /// </summary>
        [JsonPropertyName("allow_on_multiple_devices")]
        public bool? AllowOnMultipleDevices { get; set; }

        /// <summary>
        /// Only allowed to be set if `allow_on_multiple_devices` is set to true. Any number between 1-5 is acceptable.
        /// </summary>
        [JsonPropertyName("watch_count")]
        public int? WatchCount { get; set; }

        /// <summary>
        /// Only allowed to be set if `allow_on_multiple_devices` is set to true. Any number between 1-5 is acceptable.
        /// </summary>
        [JsonPropertyName("iphone_count")]
        public int? IPhoneCount { get; set; }

        /// <summary>
        /// Must be a 6 character hexadecimal value for the background color, i.e. #FFFFFF
        /// </summary>
        [JsonPropertyName("background_color")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Must be a 6 character hexadecimal value for the label color, i.e. #000000
        /// </summary>
        [JsonPropertyName("label_color")]
        public string LabelColor { get; set; }

        /// <summary>
        /// Must be a 6 character hexadecimal value for the secondary label color, i.e. #333333
        /// </summary>
        [JsonPropertyName("label_secondary_color")]
        public string LabelSecondaryColor { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_url")]
        public string SupportUrl { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_phone_number")]
        public string SupportPhoneNumber { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("support_email")]
        public string SupportEmail { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("privacy_policy_url")]
        public string PrivacyPolicyUrl { get; set; }

        /// <summary>
        /// Shows on the back of the issued NFC key
        /// </summary>
        [JsonPropertyName("terms_and_conditions_url")]
        public string TermsAndConditionsUrl { get; set; }
    }

    public class EventLogFilters
    {
        /// <summary>
        /// Must be either `mobile` or `watch`
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }

        /// <summary>
        /// Must be in ISO8601 format
        /// </summary>
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Must be in ISO8601 format
        /// </summary>
        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Must be either `issue`, `install`, `update`, `suspend`, `resume`, or `unlink`
        /// </summary>
        [JsonPropertyName("event_type")]
        public string EventType { get; set; }
    }

    public class EventLogEntry
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        // Additional properties would be included based on API response
    }

    /// <summary>
    /// Response wrapper for listing keys
    /// </summary>
    public class KeysListResponse
    {
        [JsonPropertyName("keys")]
        public List<AccessCard> Keys { get; set; } = new List<AccessCard>();
    }

    /// <summary>
    /// Response wrapper for event logs
    /// </summary>
    public class EventLogResponse
    {
        [JsonPropertyName("events")]
        public List<EventLogEntry> Events { get; set; } = new List<EventLogEntry>();
    }

    /// <summary>
    /// Pagination metadata returned by paginated API endpoints
    /// </summary>
    public class PaginationInfo
    {
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Lightweight template reference within a pass template pair
    /// </summary>
    public class PassTemplatePairInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("ex_id")]
        public string ExId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }
    }

    /// <summary>
    /// A paired iOS/Android pass template configuration
    /// </summary>
    public class PassTemplatePair
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("ex_id")]
        public string ExId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("android_template")]
        public PassTemplatePairInfo AndroidTemplate { get; set; }

        [JsonPropertyName("ios_template")]
        public PassTemplatePairInfo IosTemplate { get; set; }
    }

    /// <summary>
    /// Response wrapper for listing pass template pairs.
    /// Maps the upstream "card_template_pairs" JSON key; the C# type/property
    /// names are preserved for backward compatibility.
    /// </summary>
    public class PassTemplatePairsResponse
    {
        [JsonPropertyName("card_template_pairs")]
        public List<PassTemplatePair> PassTemplatePairs { get; set; } = new List<PassTemplatePair>();

        [JsonPropertyName("pagination")]
        public PaginationInfo Pagination { get; set; }
    }

    /// <summary>
    /// Request to create a pass template pair
    /// </summary>
    public class CreatePassTemplatePairRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("apple_card_template_id")]
        public string AppleCardTemplateId { get; set; }

        [JsonPropertyName("google_card_template_id")]
        public string GoogleCardTemplateId { get; set; }
    }

    /// <summary>
    /// A pass template reference within a ledger item's access pass
    /// </summary>
    public class LedgerItemPassTemplate
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("use_case")]
        public string UseCase { get; set; }
    }

    /// <summary>
    /// An access pass reference within a ledger item
    /// </summary>
    public class LedgerItemAccessPass
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonPropertyName("temporary")]
        public bool? Temporary { get; set; }

        [JsonPropertyName("unified_access_pass_ex_id")]
        public string UnifiedAccessPassExId { get; set; }

        [JsonPropertyName("pass_template")]
        public LedgerItemPassTemplate PassTemplate { get; set; }
    }

    /// <summary>
    /// A billing/transaction ledger entry
    /// </summary>
    public class LedgerItem
    {
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("amount")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Amount { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonPropertyName("access_pass")]
        public LedgerItemAccessPass AccessPass { get; set; }
    }

    /// <summary>
    /// Response wrapper for listing ledger items
    /// </summary>
    public class LedgerItemsResponse
    {
        [JsonPropertyName("ledger_items")]
        public List<LedgerItem> LedgerItems { get; set; } = new List<LedgerItem>();

        [JsonPropertyName("pagination")]
        public PaginationInfo Pagination { get; set; }
    }

    /// <summary>
    /// iOS In-App Provisioning preflight response
    /// </summary>
    public class IosPreflightResponse
    {
        [JsonPropertyName("provisioningCredentialIdentifier")]
        public string ProvisioningCredentialIdentifier { get; set; }

        [JsonPropertyName("sharingInstanceIdentifier")]
        public string SharingInstanceIdentifier { get; set; }

        [JsonPropertyName("cardTemplateIdentifier")]
        public string CardTemplateIdentifier { get; set; }

        [JsonPropertyName("environmentIdentifier")]
        public string EnvironmentIdentifier { get; set; }
    }

    /// <summary>
    /// A webhook configuration
    /// </summary>
    public class Webhook
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("auth_method")]
        public string AuthMethod { get; set; }

        [JsonPropertyName("subscribed_events")]
        public List<string> SubscribedEvents { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("private_key")]
        public string PrivateKey { get; set; }
    }

    /// <summary>
    /// Response wrapper for listing webhooks
    /// </summary>
    public class WebhooksResponse
    {
        [JsonPropertyName("webhooks")]
        public List<Webhook> Webhooks { get; set; } = new List<Webhook>();

        [JsonPropertyName("pagination")]
        public PaginationInfo Pagination { get; set; }
    }

    /// <summary>
    /// Parameters for creating a webhook
    /// </summary>
    public class CreateWebhookRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("auth_method")]
        public string AuthMethod { get; set; }

        [JsonPropertyName("subscribed_events")]
        public List<string> SubscribedEvents { get; set; }
    }

    /// <summary>
    /// An HID organization
    /// </summary>
    public class HIDOrg
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("full_address")]
        public string FullAddress { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }

    /// <summary>
    /// Parameters for creating an HID organization
    /// </summary>
    public class CreateHIDOrgRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("full_address")]
        public string FullAddress { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }

    /// <summary>
    /// Parameters for completing HID org registration
    /// </summary>
    public class CompleteHIDOrgRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    /// <summary>
    /// The CloudEvents data of an access pass webhook event
    /// </summary>
    public class AccessPassEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("card_template_id")]
        public string CardTemplateId { get; set; }

        [JsonPropertyName("state")]
        public AccessPassState? State { get; set; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [JsonPropertyName("employee_id")]
        public string? EmployeeId { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("start_date")]
        public DateTimeOffset? StartDate { get; set; }

        [JsonPropertyName("expiration_date")]
        public DateTimeOffset? ExpirationDate { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

        [JsonPropertyName("hid_org_id")]
        public string? HIDOrgId { get; set; }

        [JsonPropertyName("apple_user_id")]
        public string? AppleUserId { get; set; }

        [JsonPropertyName("card_templates")]
        public IReadOnlyList<AccessPassEventCardTemplate>? CardTemplates { get; set; }

        [JsonPropertyName("details")]
        public AccessPassEventDetails? Details { get; set; }

        [JsonPropertyName("devices")]
        public IReadOnlyList<AccessPassEventDevice>? Devices { get; set; }

        public class AccessPassEventDetails
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("card_template_id")]
            public string CardTemplateId { get; set; }

            [JsonPropertyName("platform")]
            public Platform Platform { get; set; }

            [JsonPropertyName("protocol")]
            public Protocol Protocol { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("card_number")]
            public string? CardNumber { get; set; }

            [JsonPropertyName("site_code")]
            public string? SiteCode { get; set; }
        }

        public class AccessPassEventCardTemplate
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("platform")]
            public Platform Platform { get; set; }

            [JsonPropertyName("protocol")]
            public Protocol Protocol { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("metadata")]
            public Dictionary<string, object>? Metadata { get; set; }

            [JsonPropertyName("hid_org_id")]
            public string? HIDOrgId { get; set; }
        }

        public class AccessPassEventDevice
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("card_template_id")]
            public string CardTemplateId { get; set; }

            [JsonPropertyName("platform")]
            public Platform Platform { get; set; }

            [JsonPropertyName("type")]
            public DeviceType Type { get; set; }

            [JsonPropertyName("protocol")]
            public Protocol Protocol { get; set; }

            [JsonPropertyName("status")]
            public DeviceStatus Status { get; set; }

            [JsonPropertyName("card_number")]
            public string? CardNumber { get; set; }

            [JsonPropertyName("site_code")]
            public string? SiteCode { get; set; }

            [JsonPropertyName("file_data")]
            public string? FileData { get; set; }
        }
    }

    /// <summary>
    /// The CloudEvents data of a credential profile webhook event
    /// </summary>
    public class CredentialProfileEvent
    {
        [JsonPropertyName("credential_profile_id")]
        string Id { get; set; }
    }

    /// <summary>
    /// The CloudEvents data of a card template webhook event
    /// </summary>
    public class CardTemplateEvent
    {
        [JsonPropertyName("card_template_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }

        [JsonPropertyName("protocol")]
        public Protocol Protocol { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

        [JsonPropertyName("hid_org_id")]
        public string HIDOrgId { get; set; }
    }

    /// <summary>
    /// A landing page configuration
    /// </summary>
    public class LandingPage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("password_protected")]
        public bool PasswordProtected { get; set; }

        [JsonPropertyName("logo_url")]
        public string LogoUrl { get; set; }
    }

    /// <summary>
    /// Parameters for creating a landing page
    /// </summary>
    public class CreateLandingPageRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("additional_text")]
        public string AdditionalText { get; set; }

        [JsonPropertyName("bg_color")]
        public string BgColor { get; set; }

        [JsonPropertyName("allow_immediate_download")]
        public bool? AllowImmediateDownload { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("is_2fa_enabled")]
        public bool? Is2faEnabled { get; set; }

        [JsonPropertyName("logo")]
        public string Logo { get; set; }
    }

    /// <summary>
    /// Parameters for updating a landing page
    /// </summary>
    public class UpdateLandingPageRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("additional_text")]
        public string AdditionalText { get; set; }

        [JsonPropertyName("bg_color")]
        public string BgColor { get; set; }

        [JsonPropertyName("allow_immediate_download")]
        public bool? AllowImmediateDownload { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("is_2fa_enabled")]
        public bool? Is2faEnabled { get; set; }

        [JsonPropertyName("logo")]
        public string Logo { get; set; }
    }

    /// <summary>
    /// A key within a credential profile
    /// </summary>
    public class CredentialProfileKey
    {
        [JsonPropertyName("ex_id")]
        public string ExId { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("keys_diversified")]
        public bool KeysDiversified { get; set; }

        [JsonPropertyName("source_key_index")]
        public int? SourceKeyIndex { get; set; }
    }

    /// <summary>
    /// A file within a credential profile
    /// </summary>
    public class CredentialProfileFile
    {
        [JsonPropertyName("ex_id")]
        public string ExId { get; set; }

        [JsonPropertyName("file_type")]
        public string FileType { get; set; }

        [JsonPropertyName("file_size")]
        public int FileSize { get; set; }

        [JsonPropertyName("communication_settings")]
        public string CommunicationSettings { get; set; }

        [JsonPropertyName("read_rights")]
        public string ReadRights { get; set; }

        [JsonPropertyName("write_rights")]
        public string WriteRights { get; set; }

        [JsonPropertyName("read_write_rights")]
        public string ReadWriteRights { get; set; }

        [JsonPropertyName("change_rights")]
        public string ChangeRights { get; set; }
    }

    /// <summary>
    /// A credential profile
    /// </summary>
    public class CredentialProfile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("aid")]
        public string Aid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("apple_id")]
        public string AppleId { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("card_storage")]
        public string CardStorage { get; set; }

        [JsonPropertyName("keys")]
        public List<CredentialProfileKey> Keys { get; set; }

        [JsonPropertyName("files")]
        public List<CredentialProfileFile> Files { get; set; }
    }

    /// <summary>
    /// A key parameter for creating a credential profile
    /// </summary>
    public class KeyParam
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("keys_diversified")]
        public bool? KeysDiversified { get; set; }

        [JsonPropertyName("source_key_index")]
        public int? SourceKeyIndex { get; set; }
    }

    /// <summary>
    /// Parameters for creating a credential profile
    /// </summary>
    public class CreateCredentialProfileRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("app_name")]
        public string AppName { get; set; }

        [JsonPropertyName("file_id")]
        public string FileId { get; set; }

        [JsonPropertyName("keys")]
        public KeyParam[] Keys { get; set; }
    }
}