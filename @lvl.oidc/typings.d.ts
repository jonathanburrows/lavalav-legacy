/// <reference path="node_modules/@types/node/index.d.ts" />
/* SystemJS module definition */
declare var module: NodeModule;
interface NodeModule {
  id: string;
}

declare var require: NodeRequire;
interface NodeRequire {
    context: any;
}
