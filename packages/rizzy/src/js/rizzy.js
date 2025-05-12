import "./rizzy-nonce"
import "./rizzy-streaming"
import "./rizzy-state";
import { ValidationService } from "aspnet-client-validation";
import "./antiforgerySnippet";

// Set up ASP.NET validation
let validation = new ValidationService();
validation.bootstrap({ watch: true });

const Rizzy = {
    validation
};

window.Rizzy = { ...(window.Rizzy || {}), ...Rizzy };

export default Rizzy;