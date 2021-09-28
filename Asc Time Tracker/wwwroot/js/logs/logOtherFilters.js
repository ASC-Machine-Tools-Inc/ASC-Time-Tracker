var savedEmpIds, savedCategory, savedJobNum, savedNotes, savedRd;

// █▄█ █▀▀ █   █▀█ █▀▀ █▀█ █▀▀
// █ █ ██▄ █▄▄ █▀▀ ██▄ █▀▄ ▄██

// Convert the saved employee ids into an array.
function empIdsSetToArray() {
    return [...savedEmpIds];
}

// Convert the saved employee ids into a comma-separated string with no emails.
function empIdsSetToString() {
    let result = "";
    let empIds = empIdsSetToArray();

    for (let i = 0; i < empIds.length; i++) {
        let empId = empIds[i];

        if (i !== 0) {
            result += ", ";
        }

        if (empId.substr(-10) === "@ascmt.com") {
            result += empId.substr(0, empId.length - 10);
        } else {
            result += empId;
        }
    }

    return result;
}

// Save the given employee ids in the right format for lookup.
function empIdsFieldToSet(empIds) {
    empIds = empIds.toLowerCase();

    if (empIds.length === 0) {  // Ignore blank field case.
        return;
    }

    // Strip spaces and split ids by comma.
    let empIdsArray = empIds.replaceAll(" ", "").split(",");

    for (let empId of empIdsArray) {
        if (empId !== "all") {
            // Append the email to the employee id if it doesn't have one.
            if (empId.substr(-10) !== "@ascmt.com") empId += "@ascmt.com";
        }

        savedEmpIds.add(empId);
    }
}