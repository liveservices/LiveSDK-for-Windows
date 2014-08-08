namespace Microsoft.Live
{
    using System;

    public enum BackgroundTransferPreferences
    {
        /// <summary>
        /// Allow transfers only when the device is using external power and has a Wi-Fi connection.
        /// This is the default setting.
        /// </summary>
        None,

        /// <summary>
        /// Allow transfers when the device is connected to external power and has a Wi-Fi or cellular connection.
        /// </summary>
        AllowCellular,

        /// <summary>
        /// Allow transfers when there is a Wi-Fi connection and the device is using battery or external power.
        /// </summary>
        AllowBattery,

        /// <summary>
        /// Allow transfers when the device is using battery or external power and has a Wi-Fi or cellular connection.
        /// </summary>
        AllowCellularAndBattery
    }
}