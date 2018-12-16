/// <reference path="logging.ts" />

import * as React from 'react';
import { render } from 'react-dom';
import * as Logging from './logging';

let _logger: Logging.ILogger = new Logging.Logger("ReactUtils")

export function mountComponent(componentType: React.ComponentClass<any>, selector: string) {
    const elements = document.querySelectorAll(selector);
    for (let i = 0; i < elements.length; i++) {
        let node = elements[i];
        let props = JSON.parse(node.getAttribute("data-props"));
        let reactElement = React.createElement(componentType, props)
 
        render(reactElement, node);
    }
}