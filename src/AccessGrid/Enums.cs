using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace AccessGrid
{
    public enum DeviceKind
    {
        [JsonStringEnumMemberName("mobile")]
        Mobile = 1,

        [JsonStringEnumMemberName("watch")]
        Watch = 2
    }

    public enum DeviceType
    {
        [JsonStringEnumMemberName("iphone")]
        iPhone = 1,

        [JsonStringEnumMemberName("apple_watch")]
        AppleWatch = 2,

        [JsonStringEnumMemberName("android_phone")]
        AndroidPhone = 3,

        [JsonStringEnumMemberName("android_watch")]
        AndroidWatch = 4
    }

    public enum Protocol
    {
        [JsonStringEnumMemberName("desfire")]
        DESFire = 1,

        [JsonStringEnumMemberName("seos")]
        SEOS = 2,

        [JsonStringEnumMemberName("smart_tap")]
        SmartTap = 3
    }

    public enum DeviceStatus
    {
        [JsonStringEnumMemberName("CredentialsCreated")]
        CredentialsCreated = 1,

        [JsonStringEnumMemberName("bundle_delivered")]
        BundleDelivered = 2,

        [JsonStringEnumMemberName("installed")]
        Installed = 3,

        [JsonStringEnumMemberName("unintstalled")]
        Unintstalled = 4,

        [JsonStringEnumMemberName("suspended")]
        Suspended = 5
    }

    public enum Platform
    {
        [JsonStringEnumMemberName("apple")]
        Apple = 1,

        [JsonStringEnumMemberName("android")]
        Android = 2
    }

    public enum AccessPassState
    {
        [JsonStringEnumMemberName("created")]
        Created = 1,

        [JsonStringEnumMemberName("active")]
        Active = 2,

        [JsonStringEnumMemberName("suspended")]
        Suspended = 3,

        [JsonStringEnumMemberName("unlink")]
        Unlink = 4,

        [JsonStringEnumMemberName("deleted")]
        Deleted = 5
    }

    public enum AccessPassEventType
    {
        [JsonStringEnumMemberName("ag.access_pass.issued")]
        Issued = 1,

        [JsonStringEnumMemberName("ag.access_pass.viewed")]
        Viewed = 2,

        [JsonStringEnumMemberName("ag.access_pass.updated")]
        Updated = 3,

        [JsonStringEnumMemberName("ag.access_pass.suspended")]
        Suspended = 4,

        [JsonStringEnumMemberName("ag.access_pass.resumed")]
        Resumed = 5,

        [JsonStringEnumMemberName("ag.access_pass.unlinked")]
        Unlinked = 6,

        [JsonStringEnumMemberName("ag.access_pass.deleted")]
        Deleted = 7,

        [JsonStringEnumMemberName("ag.access_pass.device_added")]
        DeviceAdded = 8,

        [JsonStringEnumMemberName("ag.access_pass.device_removed")]
        DeviceRemoved = 9,

        [JsonStringEnumMemberName("ag.access_pass.expired")]
        Expired = 10
    }

    public enum CardTemplateEventType
    {
        [JsonStringEnumMemberName("ag.card_template.created")]
        Created = 1,

        [JsonStringEnumMemberName("ag.card_template.updated")]
        Updated = 2,

        [JsonStringEnumMemberName("ag.card_template.request_publishing")]
        RequestPublishing = 3,

        [JsonStringEnumMemberName("ag.card_template.published")]
        Published = 4,
    }

    public enum LandingPageEventType
    {
        [JsonStringEnumMemberName("ag.landing_page.created")]
        Created = 1,

        [JsonStringEnumMemberName("ag.landing_page.updated")]
        Updated = 2,

        [JsonStringEnumMemberName("ag.landing_page.attached_to_template")]
        AttachedToTemplate = 3
    }

    public enum CredentialProfileEventType
    {
        [JsonStringEnumMemberName("ag.credential_profile.created")]
        Created = 1,

        [JsonStringEnumMemberName("ag.credential_profile.attached_to_template")]
        AttachedToTemplate = 3
    }

    public enum HIDOrgEventType
    {
        [JsonStringEnumMemberName("ag.hid_org.created")]
        Created = 1,

        [JsonStringEnumMemberName("ag.hid_org.activated")]
        Activated = 2
    }

    public enum AccountBalanceEventType
    {
        [JsonStringEnumMemberName("ag.account_balance.low")]
        Low = 1
    }
}