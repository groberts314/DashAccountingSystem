import * as React from 'react';
import * as Logging from '../common/logging';
import * as _ from 'lodash';
import { HtmlInputType } from './formutils';

interface AccountRecord {
    id: number,
    name: string
}

interface AccountCategoryList {
    category: string,
    accounts: AccountRecord[]
}

interface AssetTypeRecord {
    id: number,
    name: string
}

interface JournalEntryAccountRecord {
    accountId: number,
    accountName: string,
    assetTypeId: number,
    assetTypeName: string,
    debit: number,
    credit: number
}

interface JournalEntryAccountsProps {
    accountsList: AccountCategoryList,
    assetTypes: AssetTypeRecord[],
    accounts: JournalEntryAccountRecord[]
}

interface JournalEntryAccountsState {
    accounts: JournalEntryAccountRecord[]
}

export class JournalEntryAccounts extends React.Component<JournalEntryAccountsProps, JournalEntryAccountsState> {
    private logger: Logging.ILogger

    constructor(props: JournalEntryAccountsProps) {
        super(props);
        this.logger = new Logging.Logger('JournalEntryAccounts');
        this.logger.info('Lodash Version:', _.VERSION);

        this.state = {
            accounts: props.accounts || []
        };

        this._onAddClick = this._onAddClick.bind(this);
    }

    render() {
        return (
            <table className="table" id="accounts-table">
                <thead>
                    <tr>
                        <th>Account</th>
                        <th>Asset Type</th>
                        <th>Debit</th>
                        <th>Credit</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
                <tfoot>
                </tfoot>
            </table>
        );
    }

    _onAddClick() {

    }
}
