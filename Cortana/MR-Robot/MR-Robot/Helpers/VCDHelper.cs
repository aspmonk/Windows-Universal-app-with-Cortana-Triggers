using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace MR_Robot.Helpers
{
    public class VCDHelper
    {

        public async Task InstallVCD(string filePath)
        {
            try
            {
                Uri vcdUri = new Uri(filePath, UriKind.Absolute);
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(vcdUri);
                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
            }
            catch (Exception)
            {
                // NOT TIME NOW NIGER WILL ADD LATER !!
                throw;
            }

        }

    }
}

