import { Entity, IAggregateRoot, Required } from '../../src';

export class AstronautExamScore extends Entity implements IAggregateRoot {
    @Required() public examineeUserId: string;
    public passed: boolean;
    public score: number;

    constructor(options?: IAstronautExamScoreOptions) {
        options = options || {};
        super(options);
        this.examineeUserId = options!.examineeUserId;
        this.passed = options!.passed;
        this.score = options!.score;
    }
}

interface IAstronautExamScoreOptions {
    examineeUserId?: string;
    passed?: boolean;
    score?: number;
    id?: number;
}
