# MHF-Roadshop Editor
This is an editor, that makes adding, changing and deleting items from a Monster Hunter Frontier roadshop fast and comfortable.
This editor is inspired by Chakratos' Savemanager and tries to improve in terms of performance.

# Installation
Just download one of the releases for your platform and unpack the archive anywhere on your pc.

# Preparation
Navigate to your folder where you have unpacked the archive and locate `appsettings.json`. You will the following json:
```json
{
  "ConnectionStrings": {
    "RoadshopItemsDatabase": "Host=<host>;Database=erupe;Username=<username>;Password=<password>"
  }
}
```
Put in the required information for `<host>`, `<username>` and `<password>` and save the file.

# Usage
### Adding items
Click on `Add new item`. A side panel will open with all information needed to create a roadshop item. Click on `Add item` to add it to your database.

### Editing items
This application mainly uses Avalonia's `DataGrid`, so you can directly edit values within the grid with one exception. The name property is not part of the database table, so this cannot be edited.

### Deleting items
Just select one of the items in the grid and click `Delete item`.

### Export
You can directly export the roadshop table as if you'd export it from PgAdmin. It'll be exported as a csv file.

### Import
You can import the exported csv from either PgAdmin or this tool itself with this option. **Note that this will not append items to the current table. It'll wipe the table clean and import the items from the file as the new table content.**
