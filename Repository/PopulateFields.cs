using Sitecore.Data.Fields;
using Sitecore.SecurityModel;
using System;
using System.Reflection;
using Sitecore.Data.Items;

namespace HappyCoaching.Repository
{
    public class PopulateFields
    {
        public void PopulateSitecoreItemDynamic(object model, Item item)
        {

            if (item != null)
            {
                using (new SecurityDisabler())
                {
                    item.Editing.BeginEdit();
                    try
                    {
                        // Get the properties of the model
                        PropertyInfo[] properties = model.GetType().GetProperties();
                        var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");

                        foreach (var property in properties)
                        {
                            // Get the value of the property
                            var value = property.GetValue(model)?.ToString();

                            // Get the corresponding field in the Sitecore item
                            Field field = item.Fields[property.Name];

                            if (field != null)
                            {
                                switch (field.TypeKey)
                                {
                                    case "single-line text":
                                        field.Value = value;
                                        break;
                                    case "multi-line text":
                                        field.Value = value;
                                        break;
                                    case "image":
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            ImageField imageField = (ImageField)field;
                                            imageField.MediaID = masterDb.GetItem("/sitecore/media library/Default Website/cover").ID;
                                        }
                                        break;
                                    case "rich text":
                                        field.Value = value;
                                        break;
                                    case "general link":
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            LinkField linkField = (LinkField)field;
                                            linkField.Url = value;
                                        }
                                        break;
                                    default:
                                        field.Value = value;
                                        break;
                                }
                            }
                        }

                        item.Editing.EndEdit();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions
                        item.Editing.CancelEdit();
                        // Log the exception
                        Sitecore.Diagnostics.Log.Error("Error populating Sitecore item dynamically", ex, this);
                    }
                }
            }
            else
            {
                // Handle the case where the item is not found
                Sitecore.Diagnostics.Log.Warn($"Item with ID {item.ID} not found.", this);
            }
        }
    }
}