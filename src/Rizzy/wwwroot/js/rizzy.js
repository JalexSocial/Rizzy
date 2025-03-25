import "./rizzy-nonce"
import "./rizzy-streaming"
import { ValidationService } from "./vendor/aspnet-validation/aspnet-validation";
import "./antiforgerySnippet.min";

// Set up ASP.NET validation
let validationService = new ValidationService();
validationService.bootstrap({ watch: true });
window.validation = validationService;