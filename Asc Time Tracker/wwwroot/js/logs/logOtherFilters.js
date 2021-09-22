var savedEmpIds;

// ▀█▀ █▄ █ ▀█▀ ▀█▀ ▀█▀ ▄▀▄ █   ▀█▀ ▀██ ▄▀▄ ▀█▀ ▀█▀ █▀█ █▄ █
// ▄█▄ █ ▀█ ▄█▄  █  ▄█▄ █▀█ █▄▄ ▄█▄ ██▄ █▀█  █  ▄█▄ █▄█ █ ▀█

// TODO: load in filter data.

// █▄█ █▀▀ █   █▀█ █▀▀ █▀█ █▀▀
// █ █ ██▄ █▄▄ █▀▀ ██▄ █▀▄ ▄██

// Convert the entered employee ids into values for the set.
function empIdsFieldToSet() {
    return [...savedEmpIds];
}

// Convert a set of saved employee ids into the field value.
function empIdsSetToField(empIds) {
    empIds = empIds.toLowerCase();

    if (empIds.length === 0) {  // Ignore blank field case.
        return;
    } else if (empIds === "all") {  // Return all ids case.
        savedEmpIds.add(empIds);
        return;
    }

    // Strip spaces and split ids by comma.
    let empIdsArray = empIds.replaceAll(' ', '').split(',');

    for (let empId of empIdsArray) {
        // Append the email to the employee id if it doesn't have one.
        if (empId.substr(-10) !== "@ascmt.com") empId += "@ascmt.com";
        savedEmpIds.add(empId);
    }
}