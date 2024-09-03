using HappyCoaching.Models;
using HappyCoaching.Repository;
using HappyCoaching.Helpers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HappyCoaching.Commands
{
    public class FillValuesCommand : Command
    {
        private readonly PopulateFields populateFields;
        public FillValuesCommand()
        {
            populateFields = new PopulateFields();
        }
        public override void Execute(CommandContext context)
        {

            string apiKey = "e6dd0abdb3f34e4f99645ab1d3542a86";
            var defaultModel = new FillValueModel();
            var defaultFieldNames = defaultModel.GetType().GetProperties().Select(p => p.Name).ToList();
            Assert.ArgumentNotNull(context, "context");

            //Will get the current item from here
            Item item = context.Items[0];
            if (item != null)
            {
                // This line is neccessary to get all the fields of the item
                item.Fields.ReadAll();
                var fieldNames = item.Fields
                    .Where(f => !f.Name.StartsWith("__"))
                    .Select(f => f.Name)
                    .ToList();

                fieldNames = fieldNames.Count > 0 ? fieldNames : defaultFieldNames;

                Type dynamicType = CreateObjectOfCurrentItem.CreateDynamicType(fieldNames);
                object dynamicObject = Activator.CreateInstance(dynamicType);

                // Get the API response
                string apiResponse = Task.Run(async () => await APICall.GetApiResponse(fieldNames, apiKey)).GetAwaiter().GetResult();

                // Parse the API response
                var initialValues = APICall.ParseApiResponse(apiResponse, fieldNames);

                // Set initial values to dynamicObject properties
                SetValuesOfObject.SetValues(fieldNames, dynamicType, dynamicObject, initialValues);

                //fill values of the current item
                populateFields.PopulateSitecoreItemDynamic(dynamicObject, item);
            }
        }
    }
}
