// site.js - Handles client-side interactivity for CMCS

document.addEventListener("DOMContentLoaded", function () {

    // Auto-calculate total amount on claim form
    const hoursInput = document.querySelector("#HoursWorked");
    const rateInput = document.querySelector("#HourlyRate");
    const totalInput = document.querySelector("#TotalAmount");

    if (hoursInput && rateInput && totalInput) {
        function calculateTotal() {
            const hours = parseFloat(hoursInput.value) || 0;
            const rate = parseFloat(rateInput.value) || 0;
            totalInput.value = (hours * rate).toFixed(2);
        }

        hoursInput.addEventListener("input", calculateTotal);
        rateInput.addEventListener("input", calculateTotal);
    }

    // File upload display
    const fileInput = document.querySelector("#SupportingDocument");
    const fileLabel = document.querySelector("#FileLabel");
    if (fileInput && fileLabel) {
        fileInput.addEventListener("change", () => {
            if (fileInput.files.length > 0) {
                fileLabel.textContent = "Uploaded: " + fileInput.files[0].name;
            }
        });
    }

    // Confirmation alerts for approval/rejection buttons
    document.querySelectorAll(".btn-success, .btn-danger").forEach(btn => {
        btn.addEventListener("click", function () {
            const action = this.classList.contains("btn-success") ? "approved" : "rejected";
            alert(`Claim has been ${action}.`);
        });
    });
});
