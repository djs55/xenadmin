/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 * 
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections;
using System.Collections.Generic;

using CookComputing.XmlRpc;


namespace XenAPI
{
    public partial class Auth : XenObject<Auth>
    {
        public Auth()
        {
        }

        /// <summary>
        /// Creates a new Auth from a Proxy_Auth.
        /// </summary>
        /// <param name="proxy"></param>
        public Auth(Proxy_Auth proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Auth update)
        {
        }

        internal void UpdateFromProxy(Proxy_Auth proxy)
        {
        }

        public Proxy_Auth ToProxy()
        {
            Proxy_Auth result_ = new Proxy_Auth();
            return result_;
        }

        /// <summary>
        /// Creates a new Auth from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Auth(Hashtable table)
        {
        }

        public bool DeepEquals(Auth other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return false;
        }

        public override string SaveChanges(Session session, string opaqueRef, Auth server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }

        public static string get_subject_identifier(Session session, string _subject_name)
        {
            return (string)session.proxy.auth_get_subject_identifier(session.uuid, (_subject_name != null) ? _subject_name : "").parse();
        }

        public static Dictionary<string, string> get_subject_information_from_identifier(Session session, string _subject_identifier)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.auth_get_subject_information_from_identifier(session.uuid, (_subject_identifier != null) ? _subject_identifier : "").parse());
        }

        public static string[] get_group_membership(Session session, string _subject_identifier)
        {
            return (string [])session.proxy.auth_get_group_membership(session.uuid, (_subject_identifier != null) ? _subject_identifier : "").parse();
        }

        public static Dictionary<XenRef<Auth>, Auth> get_all_records(Session session)
        {
            return XenRef<Auth>.Create<Proxy_Auth>(session.proxy.auth_get_all_records(session.uuid).parse());
        }


    }
}
