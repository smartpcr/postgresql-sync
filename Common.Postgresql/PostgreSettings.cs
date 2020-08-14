// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSettings.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Postgresql
{
    public class PostgreSettings
    {
        public string Host { get; set; }
        public string Db { get; set; }
        public string User { get; set; }
        public string PwdSecretName { get; set; }
    }
}