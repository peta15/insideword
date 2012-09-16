using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InsideWordProvider;
using InsideWordMVCWeb.Models.WebProvider;

namespace InsideWordMVCWeb.Models.BusinessLogic
{
    public class PhotoBL
    {
        /// <summary>
        /// Function creates a ProviderPhoto class from a raw image file and assigns it to the member that created/uploaded it
        /// </summary>
        /// <param name="owningMember">owning member of the article/photo</param>
        /// <param name="filePhoto">raw image file of the photo</param>
        /// <returns>photo url</returns>
        static public ProviderPhoto CreatePhoto(ProviderMember owningMember, HttpPostedFileBase filePhoto)
        {
            ProviderPhoto originalPhoto = null;
            
            // check if there is any data to attach
            if (filePhoto.ContentLength > 0)
            {
                string subFolder = (owningMember.IsNew) ? "anonymous" : owningMember.Id.ToString();
                originalPhoto = new ProviderPhoto(filePhoto, subFolder);
                originalPhoto.AdjustToDimensions(InsideWordProvider.ProviderPhotoRecord.ImageTypeEnum.Original);
                originalPhoto.Save();

                List<ProviderPhoto> thumbnails = ProviderPhoto.CreateThumbnails(originalPhoto);
                foreach (ProviderPhoto thumbnail in thumbnails)
                {
                    thumbnail.Save();
                }
            }
            else
            {
                throw new Exception("no uploaded photo data");
            }

            return originalPhoto;
        }
    }
}