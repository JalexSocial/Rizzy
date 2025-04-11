import "./rizzy-nonce"
import "./rizzy-streaming"
import { ValidationService } from "./vendor/aspnet-validation/aspnet-validation";
import "./antiforgerySnippet.min";

// Set up ASP.NET validation
let validation = new ValidationService();
validation.bootstrap({ watch: true });

const Rizzy = {
    validation
};

window.Rizzy = { ...(window.Rizzy || {}), Rizzy };

export default Rizzy;