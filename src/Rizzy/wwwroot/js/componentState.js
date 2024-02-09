/*
Experimental
Looks within the dom for a comment node that matches the pattern 'rzstate:identifier:payload' and returns the payload, if available.
*/
function findComponentState(identifier) {
    const prefix = `rzstate:${identifier}:`;
    let payload = null;

    // Use treeWalker to iterate through all comment nodes in the entire document
    const treeWalker = document.createTreeWalker(
        document.documentElement, // Start from the root of the document
        NodeFilter.SHOW_COMMENT,
        {
            acceptNode: function (node) {
                // Check if the node value starts with the prefix
                if (node.nodeValue.trim().startsWith(prefix)) {
                    return NodeFilter.FILTER_ACCEPT;
                }
                return NodeFilter.FILTER_SKIP;
            }
        }
    );

    // Iterate through all matching comment nodes
    while (treeWalker.nextNode()) {
        const node = treeWalker.currentNode;
        // Extract the payload by removing the prefix
        payload = node.nodeValue.trim().substring(prefix.length);
        break; // Assuming only one comment node matches, we can break after finding it
    }

    return payload;
}
