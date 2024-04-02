// Function executed when the window is fully loaded
window.onload = function () {
    // Extracting information from the URL parameters
    var urlString = window.location.href;
    var url = new URL(urlString);
    var senderName = url.searchParams.get("name");
    var senderEmail = url.searchParams.get("email");
    var senderMobile = url.searchParams.get("mobile");
    var messageContent = url.searchParams.get("enquiry");

    // Constructing a message with the received sender information
    var completeMessage = "<h3><p><b>Thank you for reaching out!</b><br><br></h3>" +
        "We've received the following details:<br><br>" +
        "<b>Full Name:</b> " + senderName + "<br>" +
        "<b>Email:</b> " + senderEmail + "<br>" +
        "<b>Phone Number:</b> " + senderMobile + "<br>" +
        "<b>Message:</b> " + messageContent + "</p>";

    // Additional information to be displayed after details
    var additionalInfo = "<h3><p><b>We'll get back to you via email.</b></p><h3>";

    // Combining the complete message with additional information
    completeMessage += additionalInfo;

    // Displaying the complete message
    document.getElementById('message-received').innerHTML = completeMessage;
}
