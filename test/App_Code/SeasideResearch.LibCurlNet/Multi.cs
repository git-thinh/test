/***************************************************************************
 *
 * Project: libcurl.NET
 *
 * Copyright (c) 2004, 2005 Jeff Phillips (jeff@jeffp.net)
 *
 * This software is licensed as described in the file LICENSE.md, which you
 * should have received as part of this distribution.
 *
 * You may opt to use, copy, modify, merge, publish, distribute and/or sell
 * copies of this Software, and permit persons to whom the Software is
 * furnished to do so, under the terms of the LICENSE.md file.
 *
 * This software is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
 * ANY KIND, either express or implied.
 *
 * $Id: Multi.cs,v 1.1 2005/02/17 22:47:25 jeffreyphillips Exp $
 **************************************************************************/

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace SeasideResearch.LibCurlNet
{
	/// <summary>
	/// Implements the <c>curl_multi_xxx</c> API.
	/// </summary>
	public class Multi : IDisposable
	{
        // private members
        IntPtr      m_pMulti;
        IntPtr      m_fdSets;
        int         m_maxFD;
        MultiInfo[] m_multiInfo;
        bool        m_bGotMultiInfo;
        Hashtable   m_htEasy;
	    bool        disposed;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <exception cref="System.InvalidOperationException">This is thrown
        /// if <see cref="Curl"/> hasn't bee properly initialized.</exception>
        /// <exception cref="System.NullReferenceException">
        /// This is thrown if the native <c>Multi</c> handle wasn't
        /// created successfully.
        /// </exception>
        public Multi()
		{
            Curl.EnsureCurl();
            m_pMulti = External.curl_multi_init();
            EnsureHandle();
            m_fdSets = IntPtr.Zero;
            m_maxFD = 0;
            m_fdSets = External.curl_shim_alloc_fd_sets();
            m_multiInfo = null;
            m_bGotMultiInfo = false;
            m_htEasy = new Hashtable();
        }

	    /// <summary>
	    /// Destructor
	    /// </summary>
	    ~Multi()
	    {
	        Dispose(false);
	    }

	    /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
	    public void Dispose()
	    {
	        Dispose(true);
	        GC.SuppressFinalize(this);
	    }

	    /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed.</param>
	    protected virtual void Dispose(bool disposing)
	    {
	        if (!disposed)
	        {
	            lock (this)
	            {
	                // if (disposing) // managed member cleanup
	                // unmanaged cleanup
	                if (m_pMulti != IntPtr.Zero)
	                {
	                    External.curl_multi_cleanup(m_pMulti);
	                    m_pMulti = IntPtr.Zero;
	                }
	                if (m_fdSets != IntPtr.Zero)
	                {
	                    External.curl_shim_free_fd_sets(m_fdSets);
	                    m_fdSets = IntPtr.Zero;
	                }
	            }

	            disposed = true;
	        }
	    }

        private void EnsureHandle()
        {
            if (m_pMulti == IntPtr.Zero)
                throw new NullReferenceException("No internal multi handle");
        }

        /// <summary>
        /// Add an Easy object.
        /// </summary>
        /// <param name="easy">
        /// <see cref="Easy"/> object to add.
        /// </param>
        /// <returns>
        /// A <see cref="CURLMcode"/>, hopefully <c>CURLMcode.CURLM_OK</c>
        /// </returns>
        /// <exception cref="System.NullReferenceException">
        /// This is thrown if the native <c>Multi</c> handle wasn't
        /// created successfully.
        /// </exception>
        public CURLMcode AddHandle(Easy easy)
        {
            EnsureHandle();
            IntPtr p = easy.GetHandle();
            m_htEasy.Add(p, easy);
            return External.curl_multi_add_handle(m_pMulti,
                easy.GetHandle());
        }

        /// <summary>
        /// Remove an Easy object.
        /// </summary>
        /// <param name="easy">
        /// <see cref="Easy"/> object to remove.
        /// </param>
        /// <returns>
        /// A <see cref="CURLMcode"/>, hopefully <c>CURLMcode.CURLM_OK</c>
        /// </returns>
        /// <exception cref="System.NullReferenceException">
        /// This is thrown if the native <c>Multi</c> handle wasn't
        /// created successfully.
        /// </exception>
        public CURLMcode RemoveHandle(Easy easy)
        {
            EnsureHandle();
            IntPtr p = easy.GetHandle();
            m_htEasy.Remove(p);
            return External.curl_multi_remove_handle(m_pMulti,
                easy.GetHandle());
        }

        /// <summary>
        /// Get a string description of an error code.
        /// </summary>
        /// <param name="errorNum">
        /// The <see cref="CURLMcode"/> for which to obtain the error
        /// string description.
        /// </param>
        /// <returns>The string description.</returns>
        public String StrError(CURLMcode errorNum)
        {
            return Marshal.PtrToStringAnsi(External.curl_multi_strerror(
                errorNum));
        }

        /// <summary>
        /// Read/write data to/from each easy object.
        /// </summary>
        /// <param name="runningObjects">
        /// The number of <see cref="Easy"/> objects still in process is
        /// written by this function to this reference parameter.
        /// </param>
        /// <returns>
        /// A <see cref="CURLMcode"/>, hopefully <c>CURLMcode.CURLM_OK</c>
        /// </returns>
        /// <exception cref="System.NullReferenceException">
        /// This is thrown if the native <c>Multi</c> handle wasn't
        /// created successfully.
        /// </exception>
        public CURLMcode Perform(ref int runningObjects)
        {
            EnsureHandle();
            return External.curl_multi_perform(m_pMulti,
                ref runningObjects);
        }

        /// <summary>
        /// Set internal file desriptor information before calling Select.
        /// </summary>
        /// <returns>
        /// A <see cref="CURLMcode"/>, hopefully <c>CURLMcode.CURLM_OK</c>
        /// </returns>
        /// <exception cref="System.NullReferenceException">
        /// This is thrown if the native <c>Multi</c> handle wasn't
        /// created successfully.
        /// </exception>
        public CURLMcode FDSet()
        {
            EnsureHandle();
            return External.curl_shim_multi_fdset(m_pMulti,
                m_fdSets, ref m_maxFD);
        }

        /// <summary>
        /// Call <c>select()</c> on the Easy objects.
        /// </summary>
        /// <param name="timeoutMillis">
        /// The timeout for the internal <c>select()</c> call,
        /// in milliseconds.
        /// </param>
        /// <returns>
        /// Number or <see cref="Easy"/> objects with pending reads.
        /// </returns>
        /// <exception cref="System.NullReferenceException">
        /// This is thrown if the native <c>Multi</c> handle wasn't
        /// created successfully.
        /// </exception>
        public int Select(int timeoutMillis)
        {
            EnsureHandle();
            return External.curl_shim_select(m_maxFD + 1,
                m_fdSets, timeoutMillis);
        }

        /// <summary>
        /// Obtain status information for a Multi transfer.
        /// </summary>
        /// <returns>
        /// An array of <see cref="MultiInfo"/> objects, one for each
        /// <see cref="Easy"/> object child.
        /// </returns>
        /// <exception cref="System.NullReferenceException">
        /// This is thrown if the native <c>Multi</c> handle wasn't
        /// created successfully.
        /// </exception>
        public MultiInfo[] InfoRead()
        {
            if (m_bGotMultiInfo)
                return m_multiInfo;
            m_bGotMultiInfo = true;

            int nMsgs = 0;
            IntPtr pInfo = External.curl_shim_multi_info_read(m_pMulti,
                ref nMsgs);
            if (pInfo != IntPtr.Zero)
            {
                int msgSize = sizeof(Int32) * 2 + IntPtr.Size;
                m_multiInfo = new MultiInfo[nMsgs];
                for (int i = 0; i < nMsgs; i++)
                {
                    CURLMSG msg = (CURLMSG)Marshal.ReadInt32(
                        pInfo, i * msgSize);
                    IntPtr pEasy = Marshal.ReadIntPtr(
                        pInfo, i * msgSize + sizeof(Int32));
                    CURLcode code = (CURLcode)Marshal.ReadInt32(
                        pInfo, i * msgSize + sizeof(Int32) + IntPtr.Size);
                    m_multiInfo[i] = new MultiInfo(msg,
                        (Easy)m_htEasy[pEasy], code);
                }
                External.curl_shim_multi_info_free(pInfo);
            }
            return m_multiInfo;
        }
    }
}
