using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using InsideWordResource;
using System.Text;

namespace InsideWordProvider
{
    public class ProviderPhotoRecord : Provider
    {
        //Note that a photo doesn't mean it has a physical photo on our servers.
        //It can also just be a url link.
        protected Photo _entityPhoto;

        public ProviderPhotoRecord() : base() { }
        public ProviderPhotoRecord(long id) : base(id) { }

        public ProviderPhotoRecord(Uri AbsoluteImageUri)
        {
            Load(AbsoluteImageUri);
        }

        public ProviderPhotoRecord(ImageInfo anImageInfo)
        {
            this.ImageUrl = new Uri(anImageInfo.Src, UriKind.Absolute);
            this.DisplayWidth = anImageInfo.Width;
            this.DisplayHeight = anImageInfo.Height;
            this.Caption = anImageInfo.Alt;
            this.PhotoImageType = ImageTypeEnum.Original;
        }

        /* Prepare to delete these fields from the database
        public DateTime CreateDate
        {
            get { return _entityPhoto.CreateDate; }
            set { _entityPhoto.CreateDate = value; }
        }

        public DateTime EditDate
        {
            get { return _entityPhoto.EditDate; }
            set { _entityPhoto.EditDate = value; }
        }
        */

        public Uri ImageUrl
        {
            get { return new Uri(_entityPhoto.ImageUrl); }
            set { _entityPhoto.ImageUrl = value.AbsoluteUri; }
        }

        //Records cannot modify the Physical path
        virtual public string PhysicalPath
        {
            get { return _entityPhoto.PhysicalPath; }
            set { throw new Exception("Not a valid function"); }
        }

        public bool IsPhysicalPhoto
        {
            get { return !String.IsNullOrWhiteSpace(_entityPhoto.PhysicalPath); }
        }

        public string SubFolder
        {
            get
            {
                string folder = "";
                if (IsPhysicalPhoto)
                {
                    int length = _entityPhoto.PhysicalPath.LastIndexOf(@"\");
                    int startIndex = _entityPhoto.PhysicalPath.Substring(0, length).LastIndexOf(@"\") + 1;
                    length -= startIndex;
                    folder = _entityPhoto.PhysicalPath.Substring(startIndex, length);
                }
                return folder;
            }
        }

        public string FileName
        {
            get
            {
                string file = "";
                if (IsPhysicalPhoto)
                {
                    int startIndex = _entityPhoto.PhysicalPath.LastIndexOf(@"\") + 1;
                    file = _entityPhoto.PhysicalPath.Substring(startIndex);
                }
                return file;
            }
        }

        public string Extention
        {
            get { return System.IO.Path.GetExtension(_entityPhoto.ImageUrl); }
        }

        public string Photographer
        {
            get { return _entityPhoto.Photographer; }
            set { _entityPhoto.Photographer = value; }
        }

        public string Caption
        {
            get { return _entityPhoto.Caption; }
            set { _entityPhoto.Caption = value; }
        }

        public bool IsHidden
        {
            get { return _entityPhoto.IsHidden; }
            set { _entityPhoto.IsHidden = value; }
        }

        public short? DisplayWidth
        {
            get { return _entityPhoto.DisplayWidth; }
            set { _entityPhoto.DisplayWidth = value; }
        }

        public short? DisplayHeight
        {
            get { return _entityPhoto.DisplayHeight; }
            set { _entityPhoto.DisplayHeight = value; }
        }

        /// <summary>
        /// Indicates if image is a thumbnail of an original
        /// Note an image can only be an original OR a thumbnail
        /// thus this is the opposite of IsOriginal
        /// </summary>
        public bool IsThumbnail
        {
            get { return _entityPhoto.IsThumbnail; }
            set { _entityPhoto.IsThumbnail = value; }
        }

        public bool HasThumbnails
        {
            get { return _entityPhoto.Thumbnails.Count > 0; }
        }

        public ImageTypeEnum PhotoImageType
        {
            get { return (ImageTypeEnum)_entityPhoto.ImageType; }
            set { _entityPhoto.ImageType = (int)value; }
        }

        /// <summary>
        /// The id of the original image from which this one is copied and modified.
        /// null if it is the original image
        /// non-null long id if it is not original (ie a thumbnail)
        /// </summary>
        public long? OriginalId
        {
            get { return _entityPhoto.OriginalId; }
            set
            {
                _entityPhoto.OriginalId = value;
            }
        }

        public ProviderPhotoRecord Original
        {
            get
            {
                return new ProviderPhotoRecord(_entityPhoto.OriginalPhoto);
            }
            // TODO maybe provide a setter
        }

        public List<ProviderPhotoRecord> Thumbnails
        {
            get
            {
                return _entityPhoto.Thumbnails
                                   .ToList()
                                   .ConvertAll(_converterEntityToProvider);
            }
            // TODO maybe provide a setter
        }

        public ProviderPhotoRecord Thumbnail(ProviderPhotoRecord.ImageTypeEnum imageType)
        {
            try
            {
                return new ProviderPhotoRecord(_entityPhoto.Thumbnails.Where(photo => photo.ImageType == (int)imageType).Single());
            }
            catch
            {
                return null;
            }
        }

        public ProviderPhotoRecord CreateFakeThumbnailFromOriginal(ImageTypeEnum imageType)
        {
            if (this.IsThumbnail)
            {
                throw new Exception("warning: attempting to create thumbnail from non-original image");
            }
            ProviderPhotoRecord thumbnail = new ProviderPhotoRecord();
            thumbnail.Copy(this);
            thumbnail.OriginalId = this.Id;
            thumbnail.AdjustToDimensions(imageType);
            thumbnail.IsThumbnail = true;
            thumbnail.PhotoImageType = imageType;
            return thumbnail;
        }

        public ImageInfo PhotoInfo
        {
            get
            {
                ImageInfo photoInfo = new ImageInfo();
                photoInfo.Src = this.ImageUrl.AbsoluteUri;
                photoInfo.Height = this.DisplayHeight;
                photoInfo.Width = this.DisplayWidth;
                photoInfo.Alt = this.Caption;
                return photoInfo;
            }
        }

        // This is a utility function that will auto-resize the height and width for us
        public virtual bool AdjustToDimensions(short maxHeight, short maxWidth)
        {
            if (DisplayHeight > DisplayWidth)
            {
                if (DisplayHeight > maxHeight)
                {
                    DisplayWidth = (short)((double)DisplayWidth * (double)maxHeight / (double)DisplayHeight);
                    DisplayHeight = maxHeight;
                }
            }
            else if (DisplayWidth > maxWidth)
            {
                DisplayHeight = (short)((double)DisplayHeight * (double)maxWidth / (double)DisplayWidth);
                DisplayWidth = maxWidth;
            }

            return true;
        }

        /// <summary>
        /// Auto-resize width and height to fit within max dimensions given by ImageTypeAttributes for the given ImageType
        /// </summary>
        /// <param name="imageType"></param>
        /// <returns></returns>
        public bool AdjustToDimensions(ImageTypeEnum imageType)
        {
            return AdjustToDimensions(ProviderPhotoRecord.ImageTypeAttributes[imageType].Item1,
                                               ProviderPhotoRecord.ImageTypeAttributes[imageType].Item2);
        }

        public bool IgnoreFlags
        {
            get { return _entityPhoto.IgnoreFlags; }
            set { _entityPhoto.IgnoreFlags = value; }
        }

        override public bool Load(long id)
        {
            Photo entityPhoto = DbCtx.Instance.Photos
                                            .Where(photo => photo.Id == id)
                                            .FirstOrDefault();
            return Load(entityPhoto);
        }

        public bool Load(Uri AbsoluteImageUri)
        {
            string imgUri = AbsoluteImageUri.AbsoluteUri;
            Photo entityPhoto = DbCtx.Instance.Photos
                                    .Where(photo => photo.ImageUrl == imgUri)
                                    .FirstOrDefault();
            return Load(entityPhoto);
        }

        override public void Save()
        {
            // An original cannot be a thumbnail
            if (IsThumbnail && OriginalId == null)
            {
                throw new Exception("An original image cannot be set as a thumbnail.");
            }

            _entityPhoto.Caption = IWStringUtility.TruncateClean(_entityPhoto.Caption, CaptionSize);

            base.Save();
        }

        override public bool Delete()
        {
            bool retVal = false;
            if (!IsNew)
            {
                //Only delete if this isn't a physical file
                if (IsPhysicalPhoto)
                {
                    base.Delete();
                }
            }
            else
            {
                Clear();
                retVal = true;
            }

            return retVal;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append("\n\tId = " + _entityPhoto.Id);
            sb.Append("\n\tSystemCreateDate =\t" + _entityPhoto.SystemCreateDate);
            sb.Append("\n\tSystemEditDate =\t" + _entityPhoto.SystemEditDate);
            sb.Append("\n\tCreateDate =\t" + _entityPhoto.CreateDate);
            sb.Append("\n\tEditDate =\t" + _entityPhoto.EditDate);
            sb.Append("\n\tImageUrl =\t" + _entityPhoto.ImageUrl);
            sb.Append("\n\tPhysicalPath =\t" + _entityPhoto.PhysicalPath);
            sb.Append("\n\tIsThumbnail =\t" + _entityPhoto.IsThumbnail);
            sb.Append("\n\tImageType =\t" + _entityPhoto.ImageType);
            sb.Append("\n\tOriginalId =\t" + _entityPhoto.OriginalId);
            sb.Append("\n\tPhotographer =\t" + _entityPhoto.Photographer);
            sb.Append("\n\tCaption =\t" + _entityPhoto.Caption);
            sb.Append("\n\tIsHidden =\t" + _entityPhoto.IsHidden);
            sb.Append("\n\tDisplayWidth =\t" + _entityPhoto.DisplayWidth);
            sb.Append("\n\tDisplayHeight =\t" + _entityPhoto.DisplayHeight);
            sb.Append("\n\tIgnoreFlags =\t" + _entityPhoto.IgnoreFlags);
            sb.Append("\n");

            return sb.ToString();
        }

        public void Copy(ProviderPhotoRecord aPhotoRecord)
        {
            EntityCopy(aPhotoRecord);
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================
        protected internal ProviderPhotoRecord(Photo aPhoto) : base(aPhoto) { }

        protected override IInsideWordEntity UnderlyingEntity
        {
            get { return _entityPhoto; }
            set { _entityPhoto = (Photo)value; }
        }

        override protected void EntityClear()
        {
            _entityPhoto = new Photo();
            _entityPhoto.Id = -1;
            _entityPhoto.SystemCreateDate = new DateTime();
            _entityPhoto.SystemEditDate = new DateTime();
            _entityPhoto.CreateDate = new DateTime();
            _entityPhoto.EditDate = new DateTime();
            _entityPhoto.ImageUrl = String.Empty;
            _entityPhoto.PhysicalPath = String.Empty;
            _entityPhoto.IsThumbnail = false;
            _entityPhoto.ImageType = (int)ImageTypeEnum.Original;
            _entityPhoto.OriginalId = null;
            _entityPhoto.Photographer = String.Empty;
            _entityPhoto.Caption = String.Empty;
            _entityPhoto.IsHidden = false;
            _entityPhoto.DisplayWidth = null;
            _entityPhoto.DisplayHeight = null;
            _entityPhoto.IgnoreFlags = false;
        }

        protected bool EntityCopy(ProviderPhotoRecord aPhoto)
        {
            //Never copy over the id, otherwise we would be creating 
            //a pseudo-reference copy, which we don't want.
            //Do not copy over the system times and only the business logic
            //times since the system times are specific to a given instance.
            _entityPhoto.CreateDate = aPhoto._entityPhoto.CreateDate;
            _entityPhoto.EditDate = aPhoto._entityPhoto.EditDate;
            _entityPhoto.IsThumbnail = aPhoto._entityPhoto.IsThumbnail;
            _entityPhoto.ImageType = aPhoto._entityPhoto.ImageType;
            _entityPhoto.OriginalId = aPhoto._entityPhoto.OriginalId;
            _entityPhoto.Photographer = aPhoto._entityPhoto.Photographer;
            _entityPhoto.Caption = aPhoto._entityPhoto.Caption;
            _entityPhoto.IsHidden = aPhoto._entityPhoto.IsHidden;
            _entityPhoto.DisplayWidth = aPhoto._entityPhoto.DisplayWidth;
            _entityPhoto.DisplayHeight = aPhoto._entityPhoto.DisplayHeight;
            _entityPhoto.IgnoreFlags = aPhoto._entityPhoto.IgnoreFlags;
            _entityPhoto.ImageUrl = aPhoto._entityPhoto.ImageUrl;

            //_entityPhoto.PhysicalPath = aPhoto._entityPhoto.PhysicalPath;
            // Do not copy over the physical path. Two records should never point to the same physical path.

            return true;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        
        // when adding a new ImageType, ensure it is added to ImageTypeAttributes below
        // explicitly numbered as a reminder that the numbers will persist on images in the database
        /// <summary>
        /// Type of image, the first being the original, and the remainder generally being thumbnails in decreasing order of size
        /// </summary>
        public enum ImageTypeEnum
        {
            Original = 0, // Original image = largest
            SmallProfileThumbnail = 1,
            ProfileThumbnail = 2,
            BlurbThumbnail = 3
        }

        /// <summary>
        /// Attributes describing imageType including: ImageType, maxWidth, maxHeight
        /// </summary>
        public static Dictionary<ImageTypeEnum, Tuple<short, short>> ImageTypeAttributes = new Dictionary<ImageTypeEnum, Tuple<short, short>>()
        {
            // ImageType, maxWidth, maxHeight
            {ImageTypeEnum.Original,Tuple.Create<short,short>(640,640)}, // original has largest dimensions of any image type
            {ImageTypeEnum.SmallProfileThumbnail,Tuple.Create<short,short>(50,50)},
            {ImageTypeEnum.ProfileThumbnail,Tuple.Create<short,short>(120,120)},
            {ImageTypeEnum.BlurbThumbnail,Tuple.Create<short,short>(150,150)}
        };
        
        public const int IdDigitSize = 18;
        public const int CaptionSize = 128;

        static internal Converter<Photo, ProviderPhotoRecord> _converterEntityToProvider = new Converter<Photo, ProviderPhotoRecord>(_EntityToProvider);
        static internal ProviderPhotoRecord _EntityToProvider(Photo photoEntity)
        {
            return new ProviderPhotoRecord(photoEntity);
        }

        static public List<ProviderPhotoRecord> LoadAll()
        {
            return DbCtx.Instance.Photos.ToList().ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Load all photos for an article.  All photos can be assumed to be originals based on current
        /// implementation but use LoadArticlePhotosByArticleId if you don't want to rely on the assumption
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns>photos</returns>
        static public List<ProviderPhotoRecord> LoadByArticleId(long articleId)
        {
            return DbCtx.Instance.Photos.Where(photo => photo.Articles.Select(article => article.Id)
                                                                  .Contains(articleId))
                                            .ToList()
                                            .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Load all photos for an article and restricts them to originals
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns>photos</returns>
        static public List<ProviderPhotoRecord> LoadArticlePhotosByArticleId(long articleId)
        {
            int imgType = (int)ImageTypeEnum.Original;
            return DbCtx.Instance.Photos.Where(photo => photo.Articles.Select(article => article.Id)
                                                                  .Contains(articleId) &&
                                                             photo.IsThumbnail == false && photo.ImageType == imgType)
                                            .ToList()
                                            .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Indicates if an object with a given Id exists
        /// </summary>
        /// <param name="id">Id of the object whose existence we wish to check</param>
        /// <returns></returns>
        static public bool Exists(long id)
        {
            return id >= 0 && DbCtx.Instance.Photos.Any(aPhoto => aPhoto.Id == id);
        }

        static public List<ProviderPhotoRecord> LoadByGroupId(long groupId)
        {
            return DbCtx.Instance.Memberships
                .Where(aMembership => aMembership.GroupId == groupId)
                .Select(aMembership => aMembership.Member)
                .SelectMany(aMember => aMember.PostedPhotos)
                .ToList()
                .ConvertAll(_converterEntityToProvider);
        }

        /// <summary>
        /// Create thumbnails from a given original of size smaller than the original
        /// </summary>
        /// <param name="filePhoto"></param>
        /// <param name="subFolder">sub folder name</param>
        /// <returns>list of thumbnail photos in decreasing order of size (order of the ImageType enum)</returns>
        public static List<ProviderPhotoRecord> CreateFakeThumbnails(ProviderPhotoRecord photo)
        {
            List<ProviderPhotoRecord> thumbnailList = new List<ProviderPhotoRecord>();
            // Cannot properly create thumbnails if we don't have the width and height dimensions.
            if (photo.DisplayHeight.HasValue && photo.DisplayHeight > 1 && 
                photo.DisplayWidth.HasValue && photo.DisplayWidth > 1)
            {
                foreach (ImageTypeEnum type in Enum.GetValues(typeof(ImageTypeEnum)))
                {
                    if (type != ImageTypeEnum.Original)
                    {
                        // optionally we could detect here if the thumbnail max dimensions are larger than
                        // the original and not create the thumbnail but for now we are creating them all
                        // so we can assume all thumbnail sizes are always available even if some are much
                        // smaller than their max dimensions
                        ProviderPhotoRecord thumbnail = photo.CreateFakeThumbnailFromOriginal(type);
                        thumbnailList.Add(thumbnail);
                    }
                }
            }

            return thumbnailList;
        }
    }
}