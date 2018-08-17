// File Autogenerated by TypeRight.  DO NOT EDIT

import * as ServerObjects from "../ServerObjects"
import { callService } from "../CallServiceStuff"

/**
 * 
 * @param dict 
 */
export function Test_Anonymous(dict: { [key: string]: string }, success?: (result: { exampleProp: ServerObjects.ExampleClass, intProp: number, stringProp: string }) => void, fail?: (result: any) => void): void {
	callService("/Home/Test_Anonymous", { 
		dict: dict,
	}, success, fail);
}

/**
 * 
 * @param example 
 */
export function Test_ObjectParam(example: ServerObjects.ExampleClass, success?: (result: number) => void, fail?: (result: any) => void): void {
	callService("/Home/Test_ObjectParam", { 
		example: example,
	}, success, fail);
}

/**
 * 
 * @param dict 
 */
export function Test_ObjectReturn(dict: { [key: string]: string }, success?: (result: ServerObjects.ExampleClass) => void, fail?: (result: any) => void): void {
	callService("/Home/Test_ObjectReturn", { 
		dict: dict,
	}, success, fail);
}

/**
 * 
 */
export function TestJson(success?: (result: boolean) => void, fail?: (result: any) => void): void {
	callService("/Home/TestJson", { 
	}, success, fail);
}


