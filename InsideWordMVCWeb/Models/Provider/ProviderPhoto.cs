using System;
using System.IO;
using System.Drawing;
using System.Web;
using System.Web.Hosting;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using InsideWordResource;
using InsideWordProvider;
using InsideWordMVCWeb.Models.Utility;

namespace InsideWordMVCWeb.Models.WebProvider
{
    public class ProviderPhoto : ProviderPhotoRecord
    {
        //Note that "photo" doesn't mean there is a physical photo on our servers.
        //It can also just be a url link.
        protected Image _physicalPhoto;
        protected bool _pathChanged;
        protected string _oldPath;
        protected bool _photoChanged;
        protected Image _oldImage;
        protected bool _dimensionChanged;
        protected short _physicalHeight;
        protected short _physicalWidth;

        public ProviderPhoto() : base() { }

        public ProviderPhoto(long id) : base(id)
        {
            if (IsPhysicalPhoto)
            {
                _physicalPhoto = Image.FromFile(_entityPhoto.PhysicalPath);
            }
        }

        public ProviderPhoto(HttpPostedFileBase file, string subFolder) : base()
        {
            string extension = System.IO.Path.GetExtension(file.FileName);
            System.Drawing.Image aPhoto = System.Drawing.Image.FromStream(file.InputStream);
            string fileName = _generator.TimeUniqueAlphaNumeric() + extension;

            PhysicalPhoto = aPhoto;
            ImageUrl = ProviderPhoto.CreateAbsoluteUrl(subFolder, fileName);
            PhysicalPath = ProviderPhoto.CreatePath(subFolder, fileName);
        }

        //Unlike the PhotoRecord this class can change the physical path
        override public string PhysicalPath
        {
            get { return _entityPhoto.PhysicalPath; }
            set
            {
                //If this isn't a new photo we are creating and the path is indeed different
                //then store the old path and mark this as a new path. We store this info
                //because if for some reason the save fails we need to recover.
                if (!IsNew && value != _entityPhoto.PhysicalPath)
                {
                    _pathChanged = true;
                    _oldPath = _entityPhoto.PhysicalPath;
                }
                _entityPhoto.PhysicalPath = value;
            }
        }

        public Image PhysicalPhoto
        {
            get { return (Image)_physicalPhoto.Clone(); }
            set
            {
                //If this isn't a new photo we are creating
                //then store the old image and mark this as a new image. We store this info
                //because if for some reason the save fails we need to recover.
                if (!IsNew)
                {
                    _photoChanged = true;
                    _oldImage = _physicalPhoto;
                }

                _physicalPhoto = value;
                _physicalHeight = (short)_physicalPhoto.Height;
                _physicalWidth = (short)_physicalPhoto.Width;
            }
        }

        public short PhysicalHeight
        {
            get { return _physicalHeight; }
            set
            {
                //If this isn't a new photo we are creating and we are indeed changing the dimension
                //then store the old image and mark this as a change. We store this info
                //because if for some reason the save fails we need to recover.
                if (!IsNew && value != _physicalHeight)
                {
                    _photoChanged = true;
                    _dimensionChanged = true;
                    _oldImage = _physicalPhoto;
                }
                _physicalHeight = value;
            }
        }

        public short PhysicalWidth
        {
            get { return _physicalWidth; }
            set
            {
                //If this isn't a new photo we are creating and we are indeed changing the dimension
                //then store the old image and mark this as a change. We store this info
                //because if for some reason the save fails we need to recover.
                if (!IsNew && value != _physicalWidth)
                {
                    _photoChanged = true;
                    _dimensionChanged = true;
                    _oldImage = _physicalPhoto;
                }
                _physicalWidth = value;
            }
        }

        // This is a utility function that will auto-resize the height and width for us
        public override bool AdjustToDimensions(short maxHeight, short maxWidth)
        {
            if (IsPhysicalPhoto)
            {
                if (PhysicalHeight > PhysicalWidth)
                {
                    if (PhysicalHeight > maxHeight)
                    {
                        _dimensionChanged = true;
                        PhysicalWidth = (short)((double)PhysicalWidth * (double)maxHeight / (double)PhysicalHeight);
                        PhysicalHeight = maxHeight;
                        DisplayHeight = PhysicalHeight;
                        DisplayWidth = PhysicalWidth;
                    }
                }
                else if (PhysicalWidth > maxWidth)
                {
                    _dimensionChanged = true;
                    PhysicalHeight = (short)((double)PhysicalHeight * (double)maxWidth / (double)PhysicalWidth);
                    PhysicalWidth = maxWidth;
                    DisplayHeight = PhysicalHeight;
                    DisplayWidth = PhysicalWidth;
                }
            }
            else
            {
                base.AdjustToDimensions(maxHeight, maxWidth);
            }

            return true;
        }

        public ProviderPhoto CreateThumbnailFromOriginal(ImageTypeEnum imageType)
        {
            if (this.IsThumbnail)
            {
                throw new Exception("warning: attempting to create thumbnail from non-original image");
            }
            ProviderPhoto thumbnail = new ProviderPhoto();
            thumbnail.Copy(this);
            thumbnail.OriginalId = this.Id;
            thumbnail.AdjustToDimensions(imageType);
            thumbnail.IsThumbnail = true;
            thumbnail.PhotoImageType = imageType;
            return thumbnail;
        }

        public bool Load(int id)
        {
            bool retVal = base.Load(id);
            if (retVal)
            {
                LoadImage();
            }

            return retVal;
        }

        override public void Save()
        {
            // Attempt to save the physical image
            if ((IsNew || _pathChanged || _photoChanged) && IsPhysicalPhoto)
            {
                ImageFormat format = _physicalPhoto.RawFormat;
                if (_dimensionChanged)
                {
                    Bitmap aBitMap = new Bitmap(_physicalWidth, _physicalHeight);
                    using (Graphics redraw = Graphics.FromImage(aBitMap))
                    {
                        redraw.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        redraw.DrawImage(_physicalPhoto, 0, 0, _physicalWidth, _physicalHeight);
                    }
                    _physicalPhoto = aBitMap;
                }
                _physicalPhoto.Save(_entityPhoto.PhysicalPath, format);
                //don't delete the old image yet
            }

            //Save the record
            bool saveSucceeded = false;
            try
            {
                base.Save();
                saveSucceeded = true;
            }
            catch (Exception caughtException)
            {
                //If we failed to save the record then attempt a recovery
                //If this was a new image then there is nothing to recover so just delete the file
                if (IsNew)
                {
                    File.Delete(_entityPhoto.PhysicalPath);
                }
                else
                {
                    //We are going to attempt a rollback here of the new image files if they were created

                    //If this was a new path for the image then delete the image at the new path
                    if (_pathChanged)
                    {
                        File.Delete(_entityPhoto.PhysicalPath);
                        //Note we don't do _entityPhoto.PhysicalPath = _oldPath;
                        //this is because the class should stay in the same state as before the save
                    }

                    //If the image changed then we must revert it
                    else if (_photoChanged)
                    {
                        _oldImage.Save(_entityPhoto.PhysicalPath);
                        //Note we don't do _physicalPhoto = _oldImage;
                        //this is because the class should stay in the same state as before the save
                    }
                }
            }

            if (saveSucceeded)
            {
                if (_pathChanged)
                {
                    try
                    {
                        //If we succeeded and the file path changed then delete the image at the old path
                        File.Delete(_oldPath);
                    }
                    catch
                    {
                        //Failing to delete it is not an issue
                    }
                }

                //clear these since they aren't needed anymore
                _pathChanged = false;
                _oldPath = String.Empty;
                _photoChanged = false;
                _oldImage = null;
                _dimensionChanged = false;
            }
        }

        override public bool Delete()
        {
            if (!IsNew)
            {
                //Attempt to delete the file first if it is a physical file.
                if (IsPhysicalPhoto)
                {
                    File.Delete(_entityPhoto.PhysicalPath);
                }
                DbCtx.Instance.DeleteObject(_entityPhoto);
                DbCtx.Instance.SaveChanges();
            }
            Clear();

            return true;
        }

        /* TODO: revisit all copy functions. LEAVE THIS ONE UNCOMMENTED FOR NOW AS IT IS ACTUALLY BEING USED.*/
        public bool Copy(ProviderPhoto aPhoto)
        {
            bool retValue = EntityCopy(aPhoto);

            //Don't copy the following since these are only for change tracking
            //_pathChanged
            //_oldPath
            //_photoChanged
            //_oldImage

            if(retValue)
            {
                //two records should never point to the same physical file so setup to create a new file
                //Saving the file, like in all providers, will occur on Save().
                if (aPhoto.IsPhysicalPhoto)
                {
                    _physicalHeight = aPhoto._physicalHeight;
                    _physicalWidth = aPhoto._physicalWidth;
                    _physicalPhoto = (Image)aPhoto._physicalPhoto.Clone();

                    string extention = aPhoto.Extention;
                    string subFolder = aPhoto.SubFolder;
                    string fileName = _generator.TimeUniqueAlphaNumeric() + extention;
                    _entityPhoto.PhysicalPath = CreatePath(subFolder, fileName);
                    _entityPhoto.ImageUrl = CreateAbsoluteUrl(subFolder, fileName).AbsoluteUri;
                }
                else
                {
                    _entityPhoto.ImageUrl = aPhoto._entityPhoto.ImageUrl;
                    _entityPhoto.PhysicalPath = aPhoto._entityPhoto.PhysicalPath;
                }
            }

            return true;
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected ProviderPhoto(Photo aPhoto) : base(aPhoto)
        {
            LoadImage();
        }

        protected override void ClassClear()
        {
            base.ClassClear();
            _physicalPhoto = null;
            _pathChanged = false;
            _oldPath = String.Empty;
            _photoChanged = false;
            _oldImage = null;
            _dimensionChanged = false;
            _physicalHeight = 0;
            _physicalWidth = 0;
        }

        protected void LoadImage()
        {
            if (IsPhysicalPhoto)
            {
                _physicalPhoto = Image.FromFile(_entityPhoto.PhysicalPath);
                _physicalHeight = (short)_physicalPhoto.Height;
                _physicalWidth = (short)_physicalPhoto.Width;
            }
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static protected UniqueKeyGenerator _generator;
        static protected int _photoCounter = 0;
        public const int PhotographerSize = 32;
        public const int PhotoCaptionSize = 128;

        static protected string _photoPhysical;
        static protected Uri _photoAbsolute;
        static protected Uri _uriRoot;

        // static constructor
        static ProviderPhoto()
        {
            _generator = new UniqueKeyGenerator();
            _uriRoot = new Uri(InsideWordWebSettings.HostName);
            if (!IWStringUtility.TryUriConcat(_uriRoot, Links.Content.img.Url(), out _photoAbsolute) ||
                !IWStringUtility.TryUriConcat(_photoAbsolute, PhotoFolder, out _photoAbsolute))
            {
                throw new Exception("ProviderPhoto failed to create the _photoAbsolute path from: "+
                                    _uriRoot.AbsoluteUri + ", " + Links.Content.img.Url() + ", " + PhotoFolder);
            }
            _photoPhysical = HostingEnvironment.MapPath(Links.Content.img.Url() + "\\" + PhotoFolder);
        }

        static public Uri CreateAbsoluteUrl(string subFolder, string fileName)
        {
            Uri concateUri = null;
            IWStringUtility.TryUriConcat(_photoAbsolute, subFolder + "/" + fileName, out concateUri);
            return concateUri;
        }

        static public string CreatePath(string subFolder, string fileName)
        {
            string physicalPath = _photoPhysical + "\\" + subFolder;
            if (!Directory.Exists(physicalPath)) { Directory.CreateDirectory(physicalPath); }
            physicalPath += "\\" + fileName;
            return physicalPath;
        }

        static public bool Validate(HttpPostedFileBase file)
        {
            return file != null && ValidateFileSize(file) && ValidateMimeType(file);
        }

        static public bool ValidateMimeType(HttpPostedFileBase file)
        {
            return file != null && ValidMimeTypes.Contains(file.ContentType);
        }

        static public Boolean ValidateFileSize(HttpPostedFileBase file)
        {
            return file != null && MinByteSize < file.ContentLength && file.ContentLength <= MaxByteSize;
        }

        static public string PhotoFolder { get { return "photos"; } }

        static public int MinByteSize
        {
            get { return 128; }
        }

        static public int MaxByteSize
        {
            // 4 megabytes is the natural size limit anyways..
            // if we want to exceed this, we'll need to
            // add an httpRuntime tag for the submit page:
            // REF: http://msdn.microsoft.com/en-us/library/aa479405.aspx
            get { return 4194304; }
        }

        static public List<string> ValidTypes
        {
            get
            {
                string[] types = { "png", "gif", "jpeg", "jpg", "pjpeg" };
                return new List<string>(types);
            }
        }

        static public List<string> ValidMimeTypes
        {
            get
            {
                string[] types = { "image/png", "image/gif", "image/jpeg", "image/jpg", "image/pjpeg" };
                return new List<string>(types);
            }
        }

        static new protected Converter<Photo, ProviderPhoto> _converterEntityToProvider = new Converter<Photo, ProviderPhoto>(_EntityToProvider);
        static new protected ProviderPhoto _EntityToProvider(Photo photoEntity)
        {
            return new ProviderPhoto(photoEntity);
        }

        static new public List<ProviderPhoto> LoadAll()
        {
            return DbCtx.Instance.Photos
                                      .ToList()
                                      .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Loads ALL photos for an article regardless of being a blurb or not
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        static public List<ProviderPhoto> LoadByArticleId(int articleId)
        {
            return DbCtx.Instance.Photos.Where(photo => photo.Articles.Select(article => article.Id)
                                                                 .Contains(articleId))
                                            .ToList()
                                            .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Loads ONLY blurb photos for a given article id
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        static public List<ProviderPhoto> LoadBlurbPhotosByArticleId(int articleId)
        {
            int imgType = (int)ImageTypeEnum.BlurbThumbnail;
            return DbCtx.Instance.Photos.Where(photo => photo.Articles.Select(article => article.Id)
                                                                  .Contains(articleId) &&
                                                             photo.IsThumbnail == true && photo.ImageType == imgType)
                                            .ToList()
                                            .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Loads ONLY photos and not blurb photos for a given article id
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        static public List<ProviderPhoto> LoadArticlePhotosByArticleId(int articleId)
        {
            int imgType = (int)ImageTypeEnum.Original;
            return DbCtx.Instance.Photos.Where(photo => photo.Articles.Select(article => article.Id)
                                                                  .Contains(articleId) &&
                                                             photo.IsThumbnail == false && photo.ImageType == imgType)
                                            .ToList()
                                            .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Create thumbnails from a given original of size smaller than the original
        /// </summary>
        /// <param name="filePhoto"></param>
        /// <param name="subFolder">sub folder name</param>
        /// <returns>list of thumbnail photos in decreasing order of size (order of the ImageType enum)</returns>
        public static List<ProviderPhoto> CreateThumbnails(ProviderPhoto photo)
        {
            List<ProviderPhoto> photos = new List<ProviderPhoto>();
            foreach (ImageTypeEnum type in Enum.GetValues(typeof(ImageTypeEnum)))
            {
                if (type == ImageTypeEnum.Original)
                {
                    continue;
                }
                // optionally we could detect here if the thumbnail max dimensions are larger than
                // the original and not create the thumbnail but for now we are creating them all
                // so we can assume all thumbnail sizes are always available even if some are much
                // smaller than their max dimensions
                ProviderPhoto thumbnail = photo.CreateThumbnailFromOriginal(type);
                photos.Add(thumbnail);
            }

            return photos;
        }
    }
}