﻿/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.XenSearch
{
    public static class PropertyAccessorHelper
    {
        public static MetricUpdater MetricUpdater;

        static PropertyAccessorHelper()
        {
            MetricUpdater = new MetricUpdater();
        }

        public static string vmCpuUsageString(VM vm)
        {
            VM_metrics metrics = vm.Connection.Resolve(vm.metrics);
            if (metrics == null)
                return "";
            double sum = 0;
            int total = (int)metrics.VCPUs_number;
            for (int i = 0; i < total; i++)
            {
                sum += MetricUpdater.GetValue(vm, String.Format("cpu{0}", i.ToString()));
            }

            if (total == 0||Double.IsNaN(sum))
                return Messages.HYPHEN;
            if (total == 1)
                return String.Format(Messages.QUERY_PERCENT_OF_CPU, (sum * 100).ToString("0."));
            return String.Format(Messages.QUERY_PERCENT_OF_CPUS, ((sum * 100) / total).ToString("0."), total);
        }

        public static int vmCpuUsageRank(VM vm)
        {
            VM_metrics metrics = vm.Connection.Resolve(vm.metrics);
            if (metrics == null)
                return 0;
            double sum = 0;
            int total = (int)metrics.VCPUs_number;
            for (int i = 0; i < total; i++)
            {
                sum += MetricUpdater.GetValue(vm, String.Format("cpu{0}", i.ToString()));
            }
            return (int)Math.Round((sum * 100.0) / (double)total);
        }

        public static string vdiMemoryUsageString(VDI vdi)
        {
            long total = vdi.virtual_size;

            if (total == 0)
                return Messages.HYPHEN;

            return Util.MemorySizeString(total);
        }

        public static string vmMemoryUsageString(VM vm)
        {
            double free = MetricUpdater.GetValue(vm, "memory_internal_free");
            double total = MetricUpdater.GetValue(vm, "memory");

            if (total == 0 || Double.IsNaN(total) || Double.IsNaN(free))
                return Messages.HYPHEN;

            return String.Format(Messages.QUERY_MEMORY_USAGE, Util.MemorySizeStringWithoutUnits((total - (free * Util.BINARY_KILO))), Util.MemorySizeString(total));
        }

        public static int vmMemoryUsageRank(VM vm)
        {
            double free = MetricUpdater.GetValue(vm, "memory_internal_free");
            double total = MetricUpdater.GetValue(vm, "memory");

            return (int)Math.Round(((total - (free * Util.BINARY_KILO)) * 100.0) / total);
        }

        public static double vmMemoryUsageValue(VM vm)
        {
            double free = MetricUpdater.GetValue(vm, "memory_internal_free");
            double total = MetricUpdater.GetValue(vm, "memory");

            return total - (free * Util.BINARY_KILO);
        }

        public static string vmDiskUsageString(VM vm)
        {
            double sum = 0;
            double max = 0;
            int i = 0;
            foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
            {
                double value = MetricUpdater.GetValue(vm, String.Format("vbd_{0}_read", vbd.device)) + MetricUpdater.GetValue(vm, String.Format("vbd_{0}_write", vbd.device));
                sum += value;
                if (value > max)
                    max = value;
                i++;
            }
            if (Double.IsNaN(sum))
                return Messages.HYPHEN;
            return i == 0 ? Messages.HYPHEN : String.Format(Messages.QUERY_DATA_AVG_MAX, (sum / (Util.BINARY_KILO * i)).ToString("0."), (max / Util.BINARY_KILO).ToString("0."));
        }

        public static string vmNetworkUsageString(VM vm)
        {
            double sum = 0;
            double max = 0;
            int i = 0;
            foreach (VIF vif in vm.Connection.ResolveAll(vm.VIFs))
            {
                double value = MetricUpdater.GetValue(vm, String.Format("vif_{0}_rx", vif.device)) + MetricUpdater.GetValue(vm, String.Format("vbd_{0}_tx", vif.device));
                sum += value;
                if (value > max)
                    max = value;
                i++;
            }
            if (Double.IsNaN(sum))
                return Messages.HYPHEN;
            return i == 0 ? Messages.HYPHEN : String.Format(Messages.QUERY_DATA_AVG_MAX, (sum / (Util.BINARY_KILO * i)).ToString("0."), (max / Util.BINARY_KILO).ToString("0."));
        }

        public static string vmIPAddresses(VM vm)
        {
            VM_guest_metrics metrics = vm.Connection.Resolve(vm.guest_metrics);
            if (metrics == null)
                return Messages.HYPHEN;

            List<string> addresses = new List<string>(metrics.networks.Values);

            if (addresses.Count > 0)
                return String.Join(", ", addresses.ToArray());
            else
                return Messages.HYPHEN;
        }

        public static string hostCpuUsageString(Host host)
        {
            double sum = 0;
            if (host.host_CPUs == null)
                return Messages.HYPHEN;
            int total = host.host_CPUs.Count;
            for (int i = 0; i < total; i++)
            {
                sum += MetricUpdater.GetValue(host, String.Format("cpu{0}", i.ToString()));
            }
            if (total == 0 || Double.IsNaN(sum))
                return Messages.HYPHEN;
            if (total == 1)
                return String.Format(Messages.QUERY_PERCENT_OF_CPU, (sum * 100).ToString("0."));
            return String.Format(Messages.QUERY_PERCENT_OF_CPUS, ((sum * 100) / total).ToString("0."), total);
        }

        public static int hostCpuUsageRank(Host host)
        {
            double sum = 0;
            if (host.host_CPUs == null)
                return 0;
            int total = host.host_CPUs.Count;
            for (int i = 0; i < total; i++)
            {
                sum += MetricUpdater.GetValue(host, String.Format("cpu{0}", i.ToString()));
            }
            return (int)Math.Round((sum * 100.0) / (double)total);
        }

        public static string hostMemoryUsageString(Host host)
        {
            double free = MetricUpdater.GetValue(host, "memory_free_kib");
            double total = MetricUpdater.GetValue(host, "memory_total_kib");

            if (total == 0 || Double.IsNaN(total)|| Double.IsNaN(free))
                return Messages.HYPHEN;

            return String.Format(Messages.QUERY_MEMORY_USAGE, Util.MemorySizeStringWithoutUnits((total - free) * Util.BINARY_KILO), Util.MemorySizeString(total * Util.BINARY_KILO));
        }

        public static int hostMemoryUsageRank(Host host)
        {
            double free = MetricUpdater.GetValue(host, "memory_free_kib");
            double total = MetricUpdater.GetValue(host, "memory_total_kib");

            return (int)Math.Round(((total - free) * 100.0) / total);
        }

        public static double hostMemoryUsageValue(Host host)
        {
            double free = MetricUpdater.GetValue(host, "memory_free_kib");
            double total = MetricUpdater.GetValue(host, "memory_total_kib");

            return total - free;
        }

        public static string hostNetworkUsageString(Host host)
        {
            double sum = 0;
            double max = 0;
            int i = 0;
            foreach (PIF pif in host.Connection.ResolveAll(host.PIFs))
            {
                if (!pif.physical)
                    continue;

                double value = MetricUpdater.GetValue(host, String.Format("pif_{0}_rx", pif.device)) + MetricUpdater.GetValue(host, String.Format("vbd_{0}_tx", pif.device));
                sum += value;
                if (value > max)
                    max = value;
                i++;
            }
            if (Double.IsNaN(sum))
                return Messages.HYPHEN;
            return i == 0 ? Messages.HYPHEN : String.Format(Messages.QUERY_DATA_AVG_MAX, (sum / (Util.BINARY_KILO * i)).ToString("0."), (max / Util.BINARY_KILO).ToString("0."));
        }

        public static String GetPoolHAStatus(Pool pool)
        {
            if (!pool.ha_enabled)
                return Messages.DISABLED;

            if (pool.ha_plan_exists_for == 1)
                return String.Format(Messages.POOL_FAILURE_TOLERATE, pool.ha_plan_exists_for);

            return String.Format(Messages.POOL_FAILURES_TOLERATE, pool.ha_plan_exists_for);
        }

        public static string GetSRHAStatus(SR sr)
        {
            Pool pool = Helpers.GetPoolOfOne(sr.Connection);
            if (pool == null || pool.ha_statefiles.Length <= 0)
                return String.Empty;

            if (sr.VDIs.Contains(new XenRef<VDI>(pool.ha_statefiles[0])))
                return Messages.HA_HEARTBEAT_SR;

            return String.Empty;
        }

        public static string GetVMHAStatus(VM vm)
        {
            if (!vm.is_a_real_vm)
                return "-";
            return Helpers.RestartPriorityI18n(vm.HARestartPriority);
        }
    }
}
