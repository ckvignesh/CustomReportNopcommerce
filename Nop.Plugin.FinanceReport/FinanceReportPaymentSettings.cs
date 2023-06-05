using Nop.Core.Configuration;

namespace Nop.Plugin.FinanceReport
{
    /// <summary>
    /// Represents settings of ENETS payment plugin
    /// </summary>
    public class FinanceReportPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets payment transaction mode
        /// </summary>
        public TransactMode TransactMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string NetsMid { get; set; }
        public string B2STxnEndURL { get; set; }
        public string RequestUrl { get; set; }
    }
}
