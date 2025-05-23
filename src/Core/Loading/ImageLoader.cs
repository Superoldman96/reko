#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;

namespace Reko.Core.Loading
{
	/// <summary>
	/// Abstract base class for image loaders. These examine a raw image, and 
    /// generate an <see cref="ILoadedImage"/>.
	/// </summary>
	public abstract class ImageLoader
	{
        /// <summary>
        /// Initializes an image loader instance.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance.</param>
        /// <param name="imageLocation">Location from which the image data was loaded.</param>
        /// <param name="imgRaw">Raw image data.</param>
        public ImageLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw)
        {
            this.Services = services;
            this.ImageLocation = imageLocation;
            this.RawImage = imgRaw;
        }

        /// <summary>
        /// The <see cref="IServiceProvider"/> instance to use.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Optional loader-specific argument specified in app.config.
        /// </summary>
        public string? Argument { get; set; }

        /// <summary>
        /// The image as it appears on the storage medium before being loaded.
        /// </summary>
        public byte[] RawImage { get; }

        /// <summary>
        /// The URI from which the image was loaded from.
        /// </summary>
        public ImageLocation ImageLocation { get; }

        /// <summary>
		/// Loads the image into memory.
		/// </summary>
		/// <param name="addrLoad">Optional base address of the  image. If not specified,
        /// use the image format's default loading address. For some image types -- e.g. 
        /// archives -- the parameter is ignored.</param>
		/// <returns>An object implementing the <see cref="ILoadedImage"/> interface.</returns>
        public abstract ILoadedImage Load(Address? addrLoad);

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="addrLoad">Optional address to load the image at.</param>
        /// <param name="arch">Optional overriden architecture.</param>
        /// <returns></returns>
        public virtual ILoadedImage Load(Address? addrLoad, IProcessorArchitecture? arch)
        {
            return Load(addrLoad);
        }


        /// <summary>
        /// Creates a binary formatter for the image. The formatter is used to
        /// display the contents of the image as text.
        /// </summary>
        /// <param name="image">Image to display.</param>
        /// <returns>An <see cref="IBinaryFormatter"/> instance.</returns>
        public virtual IBinaryFormatter CreateBinaryFormatter(IBinaryImage image)
        {
            return new NullBinaryFormatter();
        }
    }
}
