﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Utility
{
    public class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

		public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusShipped = "Shipped";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";
        public const string StatusShipping = "Shipping";
        //public const string RequestCancel = "RequestCancel";

        public const string RequestStatusProcess = "RequestProcessing";
        public const string RequestStatusApproved = "RequestApproved";

        public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
		public const string PaymentStatusRejected = "Rejected";

        public const string SessionCart = "SessionShoppingCart";

        public const string NameAS = "NameAS";
        public const string NameDS = "NameDS";
        public const string PriceAS = "PriceAS";
        public const string PriceDS = "PriceDS";
        public const string DateAS = "DateAS";
        public const string DateDS = "DateDS";

    }
}
